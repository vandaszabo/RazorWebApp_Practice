using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MintaProjekt.Models;
using MintaProjekt.Services.Employees;
using MintaProjekt.Utilities;
using System.Data.SqlClient;
using System.Security.Claims;

namespace MintaProjekt.Pages
{
    [Authorize(Policy = "CanUpdateData")]
    public class UpdateEmployeeModel : PageModel
    {
        private readonly ILogger<UpdateEmployeeModel> _logger;
        private readonly IEmployeeDataService _dataAccess;
        public SelectList? EmployeeList { get; set; }

        [BindProperty]
        public Employee? SelectedEmployee { get; set; }

        [BindProperty]
        public int EmployeeID { get; set; }

        public UpdateEmployeeModel(ILogger<UpdateEmployeeModel> logger, IEmployeeDataService dataService)
        {
            _logger = logger;
            _dataAccess = dataService;
        }

        // Retrieve all employees to choose from
        public async Task<IActionResult> OnGet()
        {
            var employees = await _dataAccess.GetEmployeesAsync();

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
                ModelState.AddModelError(string.Empty, "Invalid employee selected.");
                return RedirectToPage("/Error");
            }

            try
            {
                SelectedEmployee = await _dataAccess.GetEmployeeByIDAsync(EmployeeID);

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
                _logger.LogWarning($"SelectedEmployee object is not correctly set in UpdateEmployeeModel.");
                ModelState.AddModelError(string.Empty, "Employee selection failed.");
                return Page();
            }

            try
            {
                // Get Current User's ID
                _logger.LogDebug("Try to access current User ID.");
                string userID = HttpContext.Session.GetObjectFromJson<IdentityUser>("User").Id;
                _logger.LogInformation("User ID in UpdateEmployee OnPostUpdateAsync method: {userID}", userID);

                // Invoke UpdateEmployee from EmployeeDataService
                await _dataAccess.UpdateEmployeeAsync(SelectedEmployee, userID);
                _logger.LogInformation("Successful employee update for {ID}", SelectedEmployee.EmployeeID);
                return RedirectToPage("/Employees");
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while updating the employee.");
                return Page();
            }
        }

    }

}
