using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MintaProjekt.Exeptions;
using MintaProjekt.Models;
using MintaProjekt.Pages.Base;
using MintaProjekt.Services;
using System.Data.SqlClient;

namespace MintaProjekt.Pages
{
    [Authorize(Policy = "CanAddData")]
    public class AddEmployeeModel : BasePageModel
    {
        private readonly ILogger<AddEmployeeModel> _logger;
        private readonly IEmployeeDataService _dataService;
        

        [BindProperty]
        public Employee Employee { get; set; }

        public AddEmployeeModel(ILogger<AddEmployeeModel> logger, IEmployeeDataService dataService, UserManager<IdentityUser> userManager, IHttpContextAccessor httpContextAccessor) : base(userManager)
        {
            _logger = logger;
            _dataService = dataService;
            Employee = new Employee
            {
                HireDate = DateOnly.FromDateTime(DateTime.Now)
            };
            _logger.LogInformation(httpContextAccessor.HttpContext.User.ToString());
        }

        // Create Employee
        public async Task<IActionResult> OnPostAsync()
        {
            _logger.LogInformation("{CountryCode}, {AreaCode}, {Phonenumber}", Employee.PhoneNumber.CountryCode, Employee.PhoneNumber.SelectedAreaCode, Employee.PhoneNumber.LocalPhoneNumber);

            if(UserId == null)
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
                await _dataService.AddEmployeeAsync(Employee, "userId");
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
