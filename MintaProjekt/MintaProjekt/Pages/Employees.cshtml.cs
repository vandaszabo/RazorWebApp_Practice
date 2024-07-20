using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MintaProjekt.Models;
using MintaProjekt.Services.Employees;

namespace MintaProjekt.Pages
{
    [Authorize(Policy = "CanSelectData")]
    public class EmployeesModel : PageModel
    {
        private readonly ILogger<EmployeesModel> _logger;
        private readonly IEmployeeDataService _dataAccess;
        public IEnumerable<Employee>? Employees { get; private set; }

        public EmployeesModel(ILogger<EmployeesModel> logger, IEmployeeDataService dataService)
        {
            _logger = logger;
            _dataAccess = dataService;
        }

        // Get all Employees from DB
        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                Employees = await _dataAccess.GetEmployeesAsync();
                return Page();
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while retrieving employees.");
                return Page();
            }
        }
    }
}
