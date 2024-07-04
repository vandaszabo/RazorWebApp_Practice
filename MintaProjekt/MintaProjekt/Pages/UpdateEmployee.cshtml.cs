using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MintaProjekt.Models;
using MintaProjekt.Services;

namespace MintaProjekt.Pages
{
    public class UpdateEmployeeModel : PageModel
    {
        private readonly ILogger<UpdateEmployeeModel> _logger;
        private readonly EmployeeDataService _dataService;

        public Employee? SelectedEmployee { get; set; }

        [BindProperty] // automatically bind incoming request data to properties in PageModel class
        public int EmployeeID { get; set; }

        public UpdateEmployeeModel(ILogger<UpdateEmployeeModel> logger, EmployeeDataService dataService)
        {
            _logger = logger;
            _dataService = dataService;
            SelectedEmployee = new Employee();
        }

        // Select Employee to update
        public async Task<IActionResult> OnPostSelectAsync()
        {
            if (EmployeeID <= 0)
            {
                ModelState.AddModelError(string.Empty, "Please enter a valid employee ID.");
                return Page();
            }

            try
            {
                SelectedEmployee = await _dataService.GetEmployeeByIDAsync(EmployeeID);
                if (SelectedEmployee == null)
                {
                    ModelState.AddModelError(string.Empty, $"Employee with ID {EmployeeID} not found.");
                    return Page();
                }
                return Page();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred in UpdateEmployeeModel.");
                ModelState.AddModelError(string.Empty, "An error occurred while updating the employee.");
                return Page();
            }

        }


        // Update Employee information
        public async Task<IActionResult> OnPostUpdateAsync()
        {
            if (SelectedEmployee == null)
            {
                ModelState.AddModelError(string.Empty, "Please enter valid employee data.");
                return Page();
            }

            try
            {
                await _dataService.UpdateEmployeeAsync(SelectedEmployee);
                return RedirectToPage("/Employees");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred in UpdateEmployeeModel.");
                ModelState.AddModelError(string.Empty, "An error occurred while updating the employee.");
                return Page();
            }
        }

    }

}
