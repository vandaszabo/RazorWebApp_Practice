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
        public SelectList? EmployeeList { get; set; }

        [BindProperty]
        public Employee? SelectedEmployee { get; set; }

        [BindProperty]
        public int EmployeeID { get; set; }

        public UpdateEmployeeModel(ILogger<UpdateEmployeeModel> logger, EmployeeDataService dataService)
        {
            _logger = logger;
            _dataService = dataService;
        }

        // Retrieve all employees to choose from
        public async Task<IActionResult> OnGet()
        {
            var employees = await _dataService.GetEmployeesAsync();

            if (employees == null || !employees.Any())
            {
                _logger.LogError("No employees found in the database.");
                return NotFound();
            }
            EmployeeList = new SelectList(employees, "EmployeeID", "FullName");
            return Page();
        }

        // Select Employee to update
        public async Task<IActionResult> OnPostSelectAsync()
        {
            if (EmployeeID <= 0)
            {
                _logger.LogError("Invalid employee ID.");
                ModelState.AddModelError(string.Empty, "Please enter a valid employee ID.");
                return RedirectToPage("/Error");
            }

            try
            {
                SelectedEmployee = await _dataService.GetEmployeeByIDAsync(EmployeeID);

                if (SelectedEmployee == null)
                {
                    _logger.LogWarning("Employee not found by given ID.");
                    ModelState.AddModelError(string.Empty, $"Employee with ID {EmployeeID} not found.");
                    return Page();
                }
                return Page();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred in UpdateEmployeeModel.");
                return RedirectToPage("/Error");
            }

        }


        // Update Employee information
        public async Task<IActionResult> OnPostUpdateAsync()
        {
            if (SelectedEmployee == null || SelectedEmployee.EmployeeID <= 0)
            {
                _logger.LogError($"SelectedEmployee object is not correctly set in UpdateEmployeeModel.");
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
                ModelState.AddModelError(string.Empty, "Update failed.");
                return Page();
            }
        }

    }

}
