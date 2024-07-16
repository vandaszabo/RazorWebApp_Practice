using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MintaProjekt.Exeptions;
using MintaProjekt.Models;
using MintaProjekt.Services;
using System.Data.SqlClient;
using System.Security.Claims;

namespace MintaProjekt.Pages
{
    [Authorize(Policy = "CanInsertData")]
    public class AddEmployeeModel : PageModel
    {
        private readonly ILogger<AddEmployeeModel> _logger;
        private readonly IEmployeeDataService _dataService;
        private readonly string _userID;

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
            var currentUserID = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            _logger.LogInformation("{CountryCode}, {AreaCode}, {Phonenumber}", Employee.PhoneNumber.CountryCode, Employee.PhoneNumber.SelectedAreaCode, Employee.PhoneNumber.LocalPhoneNumber);

            if(currentUserID == null)
            {
                _logger.LogError("UserId is null in AddEmployeeModel");
                ModelState.AddModelError(string.Empty, "Cannot add employee with invalid userID.");
                return Page();
            }
            if (Employee.HasInvalidProperties())
            {
                _logger.LogError("Employee object is not correctly set in AddEmployeeModel");
                ModelState.AddModelError(string.Empty, "Please provide all information.");
                return Page();
            }
            try
            {
                await _dataService.AddEmployeeAsync(Employee, currentUserID);
                _logger.LogInformation("New Employee added: {Employee}", Employee.ToString());
                return RedirectToPage("/Employees");
            }
            catch (ArgumentException)
            {
                ModelState.AddModelError(string.Empty, "Cannot create employee with given parameters.");
                return Page();
            }
            catch (SqlException)
            {
                ModelState.AddModelError(string.Empty, "Database error occurred while adding the employee.");
                return Page();
            }
            catch (NoRowsAffectedException)
            {
                ModelState.AddModelError(string.Empty, "Employee creation failed.");
                return Page();
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while adding an employee.");
                return Page();
            }
        }
    }
}
