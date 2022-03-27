using Employees.Services;
using Employees.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Employees.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IEmployeeService employeeService;
        private readonly IWebHostEnvironment environment;

        public EmployeeController(IEmployeeService employeeService, IWebHostEnvironment environment)
        {
            this.employeeService = employeeService;
            this.environment = environment;
        }

        public IActionResult Index()
        {
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> AllTimesWorked(FormFileViewModel viewModel)
        {
            var employeesVM = new List<AllProjectsViewModel>();
            if (ModelState.IsValid)
            {
                string fileName = viewModel.ExcelFile.FileName;
                string filePath = $"{environment.WebRootPath}\\{fileName}";

                using (var stream = System.IO.File.Create(filePath))
                {
                    await viewModel.ExcelFile.CopyToAsync(stream);
                }

                employeesVM = employeeService.GetAllEmployees(filePath) as List<AllProjectsViewModel>;
                return this.View(employeesVM);
            }
            return this.Redirect("Index");
        }

        [HttpPost]
        public async Task<IActionResult> LongestTimesWorked(FormFileViewModel viewModel)
        {
            var employeesVM = new List<AllProjectsViewModel>();
            if (ModelState.IsValid)
            {
                string fileName = viewModel.ExcelFile.FileName;
                string filePath = $"{environment.WebRootPath}\\{fileName}";

                using (var stream = System.IO.File.Create(filePath))
                {
                    await viewModel.ExcelFile.CopyToAsync(stream);
                }

                employeesVM = employeeService.FindLongestTime(filePath) as List<AllProjectsViewModel>;
                return this.View(employeesVM);
            }
            return this.Redirect("Index");
        }
    }
}
