using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MintaProjekt.Models;
using MintaProjekt.Services;
using MintaProjekt.Utilities;

namespace MintaProjekt.Pages
{
    [Authorize(Policy = "CanInsertData")]
    public class AddEmployeeModel : PageModel
    {
        private readonly ILogger<AddEmployeeModel> _logger;
        private readonly IEmployeeDataAccess _dataAccess;
        private readonly UserHelper _userHelper;

        [BindProperty]
        public Employee Employee { get; set; }

        public AddEmployeeModel(ILogger<AddEmployeeModel> logger, IEmployeeDataAccess dataService, UserHelper userHelper)
        {
            _logger = logger;
            _dataAccess = dataService;
            Employee = new Employee
            {
                HireDate = DateOnly.FromDateTime(DateTime.Now)
            };
            _userHelper = userHelper;
        }

        // Create Employee
        public async Task<IActionResult> OnPostAsync()
        {
            // Validate Employee object
            if (Employee.HasInvalidProperties())
            {
                _logger.LogWarning("Employee object is not correctly set in AddEmployeeModel");
                ModelState.AddModelError(string.Empty, "Please provide all information.");
                return Page();
            }

            // Add Employee
            try
            {
                // Get Current User's ID
                _logger.LogDebug("Try to access current User ID.");
                string userID = await _userHelper.GetCurrentUserIDAsync(User);
                _logger.LogInformation("User ID in AddEmployee OnPostAsync method: {userID}", userID);

                // Invoke AddEmployee from EmployeeDataService
                await _dataAccess.AddEmployeeAsync(Employee, userID);
                _logger.LogInformation("New Employee added: {Employee}", Employee.ToString());
                return RedirectToPage("/Employees");
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while adding an employee.");
                return Page();
            }
        }
    }
}
