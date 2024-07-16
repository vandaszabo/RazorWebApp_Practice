using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MintaProjekt.Exeptions;
using MintaProjekt.Services;
using System.Data.SqlClient;
using System.Security.Claims;

namespace MintaProjekt.Pages
{
    [Authorize(Policy = "CanDeleteData")]
    public class DeleteEmployeeModel : PageModel
    {
        private readonly ILogger<DeleteEmployeeModel> _logger;
        private readonly IEmployeeDataService _dataService;

        public DeleteEmployeeModel(ILogger<DeleteEmployeeModel> logger, IEmployeeDataService dataService)
        {
            _logger = logger;
            _dataService = dataService;
        }

        [BindProperty] 
        public int EmployeeID { get; set; }


        // Delete Employee
        public async Task<IActionResult> OnPostAsync()
        {
            var currentUserID = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (currentUserID == null)
            {
                _logger.LogError("UserId is null in DeleteEmployeeModel");
                ModelState.AddModelError(string.Empty, "Cannot delete employee with invalid userID.");
                return Page();
            }

            if (EmployeeID <= 0)
            {
                _logger.LogWarning("Cannot delete employee. Invalid employee ID: {EmployeeID}", EmployeeID);
                ModelState.AddModelError(string.Empty, "Please enter a valid employee ID.");
                return Page();
            }

            try
            {
                await _dataService.DeleteEmployeeAsync(EmployeeID, currentUserID);
                _logger.LogInformation("Successfully deleted employee with ID: {EmployeeID}", EmployeeID);
                return RedirectToPage("/Employees");
            }
            catch (SqlException)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while deleting the employee.");
                return Page();
            }
            catch (NoRowsAffectedException)
            {
                ModelState.AddModelError(string.Empty, "No employee found with the given ID.");
                return Page();
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while deleting the employee.");
                return Page();
            }
        }
    }
}
