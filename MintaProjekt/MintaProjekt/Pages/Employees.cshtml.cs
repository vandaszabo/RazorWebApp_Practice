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
        private readonly IEmployeeDataService _dataService;
        public IEnumerable<Employee>? Employees { get; private set; }

        [BindProperty(SupportsGet = true)] // Allows it to be set via query string
        public int PageNumber { get; set; } = 1; // Current page

        [BindProperty] // For optional set
        public int PageSize { get; set; } = 10;

        [BindProperty]
        public Pager? EmployeePager { get; private set; }

        public EmployeesModel(ILogger<EmployeesModel> logger, IEmployeeDataService dataService)
        {
            _logger = logger;
            _dataService = dataService;
        }

        // Get employees for current page
        public async Task<IActionResult> OnGetAsync()
        {
            // Validate input parameters
            if (PageNumber < 1 || PageSize < 1)
            {
                ModelState.AddModelError(string.Empty, "Invalid page number or page size.");
                return Page();
            }

            try
            {
                // Number of all employees
                int totalRecords = await _dataService.GetEmployeesCount();

                if(totalRecords <= 0)
                {
                    ModelState.AddModelError(string.Empty, "No employees found.");
                    return Page();
                }

                // Create a pager
                EmployeePager = new Pager(totalRecords, PageNumber, PageSize);

                // Calculate Offset number
                int recordSkip = (PageNumber - 1) * PageSize;

                // Get employees
                Employees = await _dataService.GetEmployeesForPage(recordSkip, PageSize);

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
