using Employees.Data;
using Employees.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Employees.Services
{
    public class EmployeeService : IEmployeeService
    {
        private Dictionary<int, List<Employee>> employeesOnSameProject;
        private Dictionary<int, List<AllProjectsViewModel>> allTimeWorkedTogether;

        public EmployeeService()
        {
            employeesOnSameProject = new Dictionary<int, List<Employee>>();
            allTimeWorkedTogether = new Dictionary<int, List<AllProjectsViewModel>>();
        }

        public IEnumerable<AllProjectsViewModel> GetAllEmployees(string filePath)
        {       
            var viewModel = new List<AllProjectsViewModel>();

            string[] lines = File.ReadAllLines(filePath);
            employeesOnSameProject = GetEmployeesOnCommonProjects(lines);
            allTimeWorkedTogether = CalculateTotalDaysOnCommonProjects();
           
            return viewModel = allTimeWorkedTogether.SelectMany(x => x.Value).ToList();
        }
        public IEnumerable<AllProjectsViewModel> FindLongestTime(string filePath)
        {
            var viewModel = new List<AllProjectsViewModel>();

            string[] lines = File.ReadAllLines(filePath);
            employeesOnSameProject = GetEmployeesOnCommonProjects(lines);
            allTimeWorkedTogether = CalculateTotalDaysOnCommonProjects();

            var findMax = allTimeWorkedTogether.SelectMany(x => x.Value).Max(y => y.TimeInDays);

            return viewModel = allTimeWorkedTogether.SelectMany(x => x.Value).Where(y => y.TimeInDays == findMax).ToList();
        }

        private Dictionary<int, List<Employee>> GetEmployeesOnCommonProjects(string[] lines)
        {
            foreach (var emp in lines)
            {
                string[] line = emp.Split(";");

                int empId = int.Parse(line[0]);
                int projectId = int.Parse(line[1]);
               
                DateTime dateFrom = DateTime.Parse(line[2], CultureInfo.InvariantCulture);
                DateTime dateTo;

                if (!DateTime.TryParse(line[3], out dateTo))
                {
                    dateTo = DateTime.UtcNow.Date;
                }
               
                var employee = new Employee
                {
                    EmployeeId = empId,
                    ProjectId = projectId,
                    DateFrom = dateFrom,
                    DateTo = dateTo,
                };

                if (!employeesOnSameProject.ContainsKey(projectId))
                {
                    employeesOnSameProject[projectId] = new List<Employee>();
                }

                employeesOnSameProject[projectId].Add(employee);
            }

            return employeesOnSameProject;
        }

        private Dictionary<int, List<AllProjectsViewModel>> CalculateTotalDaysOnCommonProjects()
        {
            foreach (var emp in employeesOnSameProject)
            {
                var infos = new List<AllProjectsViewModel>();
                if (emp.Value.Count != 1)
                {
                    var employees = emp.Value;

                    for (int curr = 0; curr < employees.Count; curr++)
                    {
                        for (int other = 0; other < employees.Count; other++)
                        {
                            var info = new AllProjectsViewModel();
                            info.EmployeeIds[0] = employees[curr].EmployeeId;
                            info.EmployeeIds[1] = 0;
                            info.ProjectId = emp.Key;

                            if (curr != other)
                            {
                                if (employees[other].DateFrom <= employees[curr].DateFrom
                                   && employees[curr].DateFrom <= employees[other].DateTo
                                   && employees[other].DateFrom <= employees[curr].DateTo
                                   && employees[curr].DateTo <= employees[other].DateTo)
                                {
                                    info.TimeInDays = (employees[curr].DateTo - employees[curr].DateFrom).Value.Days;
                                }

                                else if (employees[other].DateFrom <= employees[curr].DateFrom
                                   && employees[curr].DateFrom <= employees[other].DateTo)
                                {
                                    info.TimeInDays = (employees[other].DateTo - employees[curr].DateFrom).Value.Days;
                                }

                                else if (employees[other].DateFrom <= employees[curr].DateTo
                                   && employees[curr].DateTo <= employees[other].DateTo)
                                {
                                    info.TimeInDays = (employees[curr].DateTo - employees[other].DateFrom).Value.Days;
                                }

                                info.EmployeeIds[1] = employees[other].EmployeeId;

                                Array.Sort(info.EmployeeIds);

                                var infoExists = infos
                                    .Where(x => x.EmployeeIds[0] == info.EmployeeIds[0] &&
                                                x.EmployeeIds[1] == info.EmployeeIds[1] &&
                                                x.TimeInDays == info.TimeInDays
                                          ).FirstOrDefault();

                                if (infoExists is null)
                                {
                                    infos.Add(info);
                                }

                                if (!allTimeWorkedTogether.ContainsKey(info.ProjectId))
                                {
                                    allTimeWorkedTogether[info.ProjectId] = new List<AllProjectsViewModel>();
                                }

                                allTimeWorkedTogether[info.ProjectId] = infos;
                            }
                        }
                    }
                }
            }
            return allTimeWorkedTogether;
        }
    }
}