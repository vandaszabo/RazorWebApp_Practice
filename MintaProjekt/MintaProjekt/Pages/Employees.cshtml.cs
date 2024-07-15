using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MintaProjekt.Models;
using MintaProjekt.Services;

namespace MintaProjekt.Pages
{
    public class EmployeesModel : PageModel
    {
        private readonly ILogger<EmployeesModel> _logger;
        private readonly IEmployeeDataService _dataService;
        public IEnumerable<Employee>? Employees { get; private set; }

        public EmployeesModel(ILogger<EmployeesModel> logger, IEmployeeDataService dataService)
        {
            _logger = logger;
            _dataService = dataService;
        }

        // Get all Employees from DB
        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                Employees = await _dataService.GetEmployeesAsync();
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while retrieving employees: {ex.Message}");
                return RedirectToPage("/Error");
            }
        }
    }
}
