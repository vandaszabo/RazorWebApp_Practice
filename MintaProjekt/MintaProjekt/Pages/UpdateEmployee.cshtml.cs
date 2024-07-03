using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MintaProjekt.Models;
using MintaProjekt.Services;

namespace MintaProjekt.Pages
{
    public class UpdateEmployeeModel : PageModel
    {
        private readonly ILogger<UpdateEmployeeModel> _logger;
        private readonly DataService _dataService;
        private IEnumerable<Employee> _employees; // Hold employees locally in the model

        [BindProperty(SupportsGet = true)] // Support getting parameters from query string
        public int EmployeeID { get; set; }

        [BindProperty]
        public Employee Employee { get; set; }

        public SelectList EmployeesSelectList { get; set; }

        public UpdateEmployeeModel(ILogger<UpdateEmployeeModel> logger, DataService dataService)
        {
            _logger = logger;
            _dataService = dataService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            _employees = await _dataService.GetEmployeesAsync();

            if (_employees == null || !_employees.Any())
            {
                _logger.LogWarning("No employees found in the database.");
                EmployeesSelectList = new SelectList(new List<Employee>(), nameof(Employee.EmployeeID), nameof(Employee.FirstName));
            }
            else
            {
                EmployeesSelectList = new SelectList(_employees, nameof(Employee.EmployeeID), nameof(Employee.FirstName));

                if (EmployeeID > 0)
                {
                    Employee = _employees.FirstOrDefault(e => e.EmployeeID == EmployeeID);
                }
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (Employee == null)
            {
                ModelState.AddModelError(string.Empty, "Please select a valid employee.");
                EmployeesSelectList = new SelectList(_employees, nameof(Employee.EmployeeID), nameof(Employee.FirstName));
                return Page();
            }

            if (!ModelState.IsValid)
            {
                EmployeesSelectList = new SelectList(_employees, nameof(Employee.EmployeeID), nameof(Employee.FirstName));
                return Page();
            }

            try
            {
                await _dataService.UpdateEmployeeAsync(Employee);
                return RedirectToPage("/Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred in UpdateEmployeeModel.");
                ModelState.AddModelError(string.Empty, "An error occurred while updating the employee.");
                EmployeesSelectList = new SelectList(_employees, nameof(Employee.EmployeeID), nameof(Employee.FirstName));
                return Page();
            }
        }
    }

}
