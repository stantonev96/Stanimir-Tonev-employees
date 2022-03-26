using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Excel;

namespace Sandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            var employeesOnSameProject = new Dictionary<int, List<Employee>>();

            string[] lines = File.ReadAllLines(
                Directory.GetCurrentDirectory().ToString() + "\\employees.csv");

            foreach (var emp in lines)
            {
                string[] line = emp.Split(";");

                int empId = int.Parse(line[0]);
                int projectId = int.Parse(line[1]);

                string[] dateFromParts = line[2].Split("-");
                int yearFrom = int.Parse(dateFromParts[0]);
                int monthFrom = int.Parse(dateFromParts[1]);
                int dayFrom = int.Parse(dateFromParts[2]);
                DateTime dateFrom = new DateTime(yearFrom, monthFrom, dayFrom);

                DateTime dateTo;
                if (line[3].Contains("-"))
                {
                    string[] dateToParts = line[3].Split("-");
                    int yearTo = int.Parse(dateToParts[0]);
                    int monthTo = int.Parse(dateToParts[1]);
                    int dayTo = int.Parse(dateToParts[2]);
                    dateTo = new DateTime(yearTo, monthTo, dayTo);
                }
                else
                {
                    dateTo = DateTime.UtcNow;
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

            foreach (var emp in employeesOnSameProject)
            {
                Console.WriteLine($"Project {emp.Key}");

                foreach (var currEmpOnProj in emp.Value)
                {
                    Console.WriteLine($"{currEmpOnProj.EmployeeId}, {currEmpOnProj.DateFrom}, " +
                        $"{currEmpOnProj.DateTo}");
                }
            }

            var longestTimeWorkedTogether = new Dictionary<int, List<Employee>>();

            foreach (var emp in employeesOnSameProject)
            {
                if (emp.Value.Count != 1)
                {
                    var employees = emp.Value;
                    for (int curr = 0; curr < emp.Value.Count; curr++)
                    {
                        for (int other = 0; other < emp.Value.Count; other++)
                        {
                            if (curr != other)
                            {
                                int timeInDays;

                                if(employees[other].DateFrom <= employees[curr].DateFrom
                                   && employees[curr].DateFrom <= employees[other].DateTo
                                   && employees[other].DateFrom <= employees[curr].DateTo
                                   && employees[curr].DateTo <= employees[other].DateTo)
                                {
                                   timeInDays = (employees[curr].DateTo - employees[curr].DateFrom).Value.Days;
                                }

                                else if (employees[other].DateFrom <= employees[curr].DateFrom
                                   && employees[curr].DateFrom <= employees[other].DateTo)
                                {
                                    timeInDays = (employees[other].DateTo - employees[curr].DateFrom).Value.Days;

                                }

                                else if(employees[other].DateFrom <= employees[curr].DateTo
                                   && employees[curr].DateTo <= employees[other].DateTo)
                                {
                                    timeInDays = (employees[curr].DateTo - employees[other].DateFrom).Value.Days;
                                }
                            }

                        }
                    }
                }
            }
        }
    }
}
