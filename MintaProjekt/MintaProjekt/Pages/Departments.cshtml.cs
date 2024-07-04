using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MintaProjekt.Models;
using MintaProjekt.Services;

namespace MintaProjekt.Pages
{
    public class DepartmentsModel : PageModel
    {
        private readonly DepartmentDataService _dataService;
        private readonly ILogger<DepartmentDataService> _logger;
        public IEnumerable<Department> Departments { get; private set; }

        [BindProperty] // automatically bind incoming request data to properties in PageModel class
        public int NewLeaderID { get; set; }

        public DepartmentsModel(ILogger<DepartmentDataService> logger , DepartmentDataService dataService)
        {
            _logger = logger;
            _dataService = dataService;
        }

        // Get all Departments from DB
        public async Task OnGetAsync()
        {
            try
            {
                Departments = await _dataService.GetDepartmentsWithEmployeesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while retrieving departments: {ex.Message}");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (NewLeaderID <= 0)
            {
                ModelState.AddModelError(string.Empty, "Please enter a valid employee ID.");
                return Page();
            }
            try
            {
                await _dataService.UpdateDepartmentLeaderAsync(NewLeaderID);
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred in AddEmployeeModel.");
                return Page();
            }
            return RedirectToPage(); // Redirect back to the Departments page
        }
    }
}
