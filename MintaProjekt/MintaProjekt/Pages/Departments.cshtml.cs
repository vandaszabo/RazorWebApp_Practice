using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MintaProjekt.Models;
using MintaProjekt.Services;
using System.Reflection;

namespace MintaProjekt.Pages
{
    public class DepartmentsModel : PageModel
    {
        private readonly DepartmentDataService _dataService;
        private readonly ILogger<DepartmentDataService> _logger;
        public IEnumerable<Department>? Departments { get; private set; }

        [BindProperty]
        public int? NewLeaderID { get; set; }
        [BindProperty] 
        public int DepartmentID { get; set; }


        public DepartmentsModel(ILogger<DepartmentDataService> logger , DepartmentDataService dataService)
        {
            _logger = logger;
            _dataService = dataService;
        }

        // Get all Departments from DB
        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                Departments = await _dataService.GetDepartmentsWithEmployeesAsync();
                _logger.LogInformation($"New leader ID in OnGetAsync: {NewLeaderID}");
                return Page();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while retrieving departments: {ex.Message}");
                return Page();
            }
        }


        // Update Department leader
        public async Task<IActionResult> OnPostAsync()
        {
            if (NewLeaderID == -1)
            {
                NewLeaderID = null;
            }

            if (NewLeaderID.HasValue && NewLeaderID <= 0)
            {
                ModelState.AddModelError(string.Empty, "Please enter a valid employee ID.");
                _logger.LogError("Invalid employee ID.");
                return await OnGetAsync(); // Reload departments and return page
            }

            try
            {
                await _dataService.UpdateDepartmentLeaderAsync(DepartmentID, NewLeaderID);
                return await OnGetAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred in DepartmentModel.");
                ModelState.AddModelError(string.Empty, "An error occurred while updating the department leader.");
                return await OnGetAsync();
            }
        }
    }
}
