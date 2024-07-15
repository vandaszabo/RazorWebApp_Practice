using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MintaProjekt.Exeptions;
using MintaProjekt.Pages.Base;
using MintaProjekt.Services;
using System.Data.SqlClient;

namespace MintaProjekt.Pages
{
    [Authorize(Policy = "CanDeleteData")]
    public class DeleteEmployeeModel : BasePageModel
    {
        private readonly ILogger<DeleteEmployeeModel> _logger;
        private readonly IEmployeeDataService _dataService;

        public DeleteEmployeeModel(ILogger<DeleteEmployeeModel> logger, IEmployeeDataService dataService, UserManager<IdentityUser> userManager) : base(userManager)
        {
            _logger = logger;
            _dataService = dataService;
        }

        [BindProperty] 
        public int EmployeeID { get; set; }


        // Delete Employee
        public async Task<IActionResult> OnPostAsync()
        {
            if (UserId == null)
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
                await _dataService.DeleteEmployeeAsync(EmployeeID, UserId);
                _logger.LogInformation("Successfully deleted employee with ID: {EmployeeID}", EmployeeID);
                return RedirectToPage("/Employees");
            }
            catch (NoRowsAffectedException)
            {
                ModelState.AddModelError(string.Empty, "No employee found with the given ID.");
                return Page();
            }
            catch (SqlException)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while deleting the employee.");
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
