using Employees.ValidationAttributes;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Employees.ViewModels
{
    public class AllProjectsViewModel
    {
        public AllProjectsViewModel()
        {
            EmployeeIds = new int[2];
        }

        public int[] EmployeeIds { get; set; }

        public int ProjectId { get; set; }

        public int TimeInDays { get; set; }   
    }
}
