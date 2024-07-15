using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MintaProjekt.Models;
using MintaProjekt.Services;
using MintaProjekt.Utilities;

namespace MintaProjekt.Pages
{
    public class AddEmployeeModel : PageModel
    {
        private readonly ILogger<AddEmployeeModel> _logger;
        private readonly IEmployeeDataService _dataService;

        [BindProperty]
        public Employee Employee { get; set; }

        public AddEmployeeModel(ILogger<AddEmployeeModel> logger, IEmployeeDataService dataService)
        {
            _logger = logger;
            _dataService = dataService;
            Employee = new Employee
            {
                HireDate = DateOnly.FromDateTime(DateTime.Now)
            };
        }

        // Create Employee
        public async Task<IActionResult> OnPostAsync()
        {
            _logger.LogInformation("{CountryCode}, {AreaCode}, {Phonenumber}", Employee.PhoneNumber.CountryCode, Employee.PhoneNumber.SelectedAreaCode, Employee.PhoneNumber.LocalPhoneNumber);
            if (Employee.HasInvalidProperties())
            {
                _logger.LogError("Employee object is not correctly set in AddEmployeeModel");
                ModelState.AddModelError(string.Empty, "Please provide all information.");
                return Page();
            }
            try
            {
                await _dataService.AddEmployeeAsync(Employee, AppUser.ID);
                _logger.LogInformation("New Employee added: {Employee}", Employee.ToString());
                return RedirectToPage("/Employees");
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
