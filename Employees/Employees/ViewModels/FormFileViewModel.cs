using Employees.ValidationAttributes;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Employees.ViewModels
{
    public class FormFileViewModel
    {
        [Required(ErrorMessage = "Please select a file.")]
        [Display(Name = "Choose Excel File: ")]
        [AllowedExtensions(new string[] { ".csv", "xlsx" })]
        public IFormFile ExcelFile { get; set; }
    }
}
