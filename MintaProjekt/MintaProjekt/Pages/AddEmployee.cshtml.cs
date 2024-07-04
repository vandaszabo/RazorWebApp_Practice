using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MintaProjekt.Models;
using MintaProjekt.Services;

namespace MintaProjekt.Pages
{
    public class AddEmployeeModel : PageModel
    {
        private readonly ILogger<AddEmployeeModel> _logger;
        private readonly EmployeeDataService _dataService;

        [BindProperty] // automatically bind incoming request data to properties in PageModel class
        public Employee Employee { get; set; }

        public AddEmployeeModel(ILogger<AddEmployeeModel> logger, EmployeeDataService dataService)
        {
            _logger = logger;
            _dataService = dataService;
            Employee = new Employee(); // Initialize the Employee property
        }


        // Create Employee
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            try
            {
                await _dataService.AddEmployeeAsync(Employee);
                return RedirectToPage("/Employees"); // Redirect to the index page after adding the employee
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred in AddEmployeeModel.");
                return Page();
            }
        }
    }
}
