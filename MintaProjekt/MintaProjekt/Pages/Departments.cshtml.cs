using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MintaProjekt.Models;
using MintaProjekt.Services.Departments;

namespace MintaProjekt.Pages
{
    public class DepartmentsModel : PageModel
    {
        private readonly IDepartmentDataService _dataAccess;
        private readonly ILogger<DepartmentDataService> _logger;
        public IEnumerable<Department>? Departments { get; private set; }

        [BindProperty]
        public int NewLeaderID { get; set; }

        [BindProperty]
        public int LeaderIDToDelete { get; set; }

        [BindProperty]
        public int DepartmentID { get; set; }


        public DepartmentsModel(ILogger<DepartmentDataService> logger, IDepartmentDataService dataService)
        {
            _logger = logger;
            _dataAccess = dataService;
        }

        // Get all Departments from DB
        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                Departments = await _dataAccess.GetDepartmentsWithEmployeesAsync();
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while retrieving departments: {ex.Message}");
                return RedirectToPage("/Error");
            }
        }


        // Manage Department leaders
        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (NewLeaderID != 0)
                {
                    _logger.LogDebug("New Leader: {ID}", NewLeaderID);
                    _logger.LogInformation("Try to add new leader.");
                    await _dataAccess.AddDepartmentLeaderAsync(DepartmentID, NewLeaderID);
                }
                else if (LeaderIDToDelete != 0)
                {
                    _logger.LogDebug("Leader to delete: {ID}", LeaderIDToDelete);
                    _logger.LogInformation("Try to delete existing leader.");
                    await _dataAccess.DeleteDepartmentLeaderAsync(DepartmentID, LeaderIDToDelete);
                }

                return await OnGetAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred in DepartmentModel.");
                ModelState.AddModelError(string.Empty, "An error occurred while updating the department leader.");
                return RedirectToPage("/Error");
            }


        }
    }
}
