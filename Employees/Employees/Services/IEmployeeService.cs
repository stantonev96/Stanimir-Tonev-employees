using Employees.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Employees.Services
{
    public interface IEmployeeService
    {
        IEnumerable<AllProjectsViewModel> GetAllEmployees(string filePath);

        IEnumerable<AllProjectsViewModel> FindLongestTime(string filePath);
    }
}
