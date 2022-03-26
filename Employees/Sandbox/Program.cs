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

            var allTimeWorkedTogether = new Dictionary<int, List<Info>>();
            var longestTimeWorkedTogether = new List<Info>();

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

            foreach (var emp in employeesOnSameProject)
            {
                Console.WriteLine($"Project {emp.Key}");

                foreach (var currEmpOnProj in emp.Value)
                {
                    Console.WriteLine($"{currEmpOnProj.EmployeeId}, {currEmpOnProj.DateFrom.ToShortDateString()}, " +
                        $"{currEmpOnProj.DateTo.Value.ToShortDateString()}");
                }
            }

            foreach (var emp in employeesOnSameProject)
            {
                var infos = new List<Info>();
                if (emp.Value.Count != 1)
                {
                    var employees = emp.Value;

                    for (int curr = 0; curr < employees.Count; curr++)
                    {
                        for (int other = 0; other < employees.Count; other++)
                        {
                            var info = new Info();
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
                                    info.LongestTimeInDays = (employees[curr].DateTo - employees[curr].DateFrom).Value.Days;
                                }

                                else if (employees[other].DateFrom <= employees[curr].DateFrom
                                   && employees[curr].DateFrom <= employees[other].DateTo)
                                {
                                    info.LongestTimeInDays = (employees[other].DateTo - employees[curr].DateFrom).Value.Days;
                                }

                                else if (employees[other].DateFrom <= employees[curr].DateTo
                                   && employees[curr].DateTo <= employees[other].DateTo)
                                {
                                    info.LongestTimeInDays = (employees[curr].DateTo - employees[other].DateFrom).Value.Days;
                                }

                                info.EmployeeIds[1] = employees[other].EmployeeId;

                                Array.Sort(info.EmployeeIds);

                                var infoExists = infos
                                    .Where(x => x.EmployeeIds[0] == info.EmployeeIds[0] &&
                                                x.EmployeeIds[1] == info.EmployeeIds[1] &&
                                                x.LongestTimeInDays == info.LongestTimeInDays
                                          ).FirstOrDefault();

                                if (infoExists is null)
                                {
                                    infos.Add(info);
                                }

                                if (!allTimeWorkedTogether.ContainsKey(info.ProjectId))
                                {
                                    allTimeWorkedTogether[info.ProjectId] = new List<Info>();
                                }

                                allTimeWorkedTogether[info.ProjectId] = infos;
                            }
                        }
                    }
                }
            }

            Console.WriteLine();

            foreach (var emp in allTimeWorkedTogether)
            {
                foreach (var k in emp.Value)
                {
                    Console.WriteLine($"{emp.Key} {string.Join(" ", k.EmployeeIds)} {k.LongestTimeInDays}");
                }
            }

            var findMax = allTimeWorkedTogether.SelectMany(x => x.Value).Max(y => y.LongestTimeInDays);

            longestTimeWorkedTogether = allTimeWorkedTogether.SelectMany(x => x.Value).Where(y => y.LongestTimeInDays == findMax).ToList();
        }
    }
}