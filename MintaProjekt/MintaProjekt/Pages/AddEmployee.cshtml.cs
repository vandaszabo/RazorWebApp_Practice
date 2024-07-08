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

        [BindProperty]
        public Employee Employee { get; set; }

        [BindProperty]
        public string? PhoneNumber { get; set; }

        [BindProperty]
        public string? SelectedAreaCode { get; set; }

        public AddEmployeeModel(ILogger<AddEmployeeModel> logger, EmployeeDataService dataService)
        {
            _logger = logger;
            _dataService = dataService;
            // Initialize the Employee property 
            Employee = new Employee{
                HireDate = DateOnly.FromDateTime(DateTime.Now)
            };
        }

        // Create Employee
        public async Task<IActionResult> OnPostAsync()
        {
            if (!string.IsNullOrEmpty(SelectedAreaCode) && !string.IsNullOrEmpty(PhoneNumber))
            {
                Employee.PhoneNumber = $"+36{SelectedAreaCode}{PhoneNumber}";
            }

            if (Employee.HasInvalidProperties())
            {
                _logger.LogError("Employee object is not correctly set in AddEmployeeModel");
                ModelState.AddModelError(string.Empty, "Please provide all information.");
                return Page();
            }
            try
            {
                await _dataService.AddEmployeeAsync(Employee);
                _logger.LogInformation($"New Employee added with ID: {Employee.EmployeeID}, Name: {Employee.FullName}");
                return RedirectToPage("/Employees"); // Redirect to the index page after adding the employee
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred in AddEmployeeModel.");
                ModelState.AddModelError(string.Empty, "An error occurred while adding an employee.");
                return Page();
            }
        }
    }
}
