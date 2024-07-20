using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MintaProjekt.Services.Employees;
using MintaProjekt.Utilities;

namespace MintaProjekt.Pages
{
    [Authorize(Policy = "CanDeleteData")]
    public class DeleteEmployeeModel : PageModel
    {
        private readonly ILogger<DeleteEmployeeModel> _logger;
        private readonly IEmployeeDataService _dataAccess;

        public DeleteEmployeeModel(ILogger<DeleteEmployeeModel> logger, IEmployeeDataService dataService)
        {
            _logger = logger;
            _dataAccess = dataService;
        }

        [BindProperty] 
        public int EmployeeID { get; set; }


        // Delete Employee
        public async Task<IActionResult> OnPostAsync()
        {
            if (EmployeeID <= 0)
            {
                _logger.LogWarning("Cannot delete employee. Invalid employee ID: {EmployeeID}", EmployeeID);
                ModelState.AddModelError(string.Empty, "Please enter a valid employee ID.");
                return Page();
            }

            try
            {
                // Get Current User's ID
                _logger.LogDebug("Try to access current User ID.");
                string userID = HttpContext.Session.GetObjectFromJson<IdentityUser>("User").Id;
                _logger.LogInformation("User ID in DeleteEmployee OnPostAsync method: {userID}", userID);

                // Invoke DeleteEmployee from EmployeeDataService
                await _dataAccess.DeleteEmployeeAsync(EmployeeID, userID);
                _logger.LogInformation("Successfully deleted employee with ID: {EmployeeID}", EmployeeID);
                return RedirectToPage("/Employees");
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while deleting the employee.");
                return Page();
            }
        }
    }
}
