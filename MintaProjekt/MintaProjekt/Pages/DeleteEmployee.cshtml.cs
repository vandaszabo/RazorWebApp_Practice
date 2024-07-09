using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MintaProjekt.Services;

namespace MintaProjekt.Pages
{
    public class DeleteEmployeeModel : PageModel
    {
        private readonly ILogger<DeleteEmployeeModel> _logger;
        private readonly EmployeeDataService _dataService;

        public DeleteEmployeeModel(ILogger<DeleteEmployeeModel> logger, EmployeeDataService dataService)
        {
            _logger = logger;
            _dataService = dataService;
        }

        [BindProperty] // automatically bind incoming request data to properties in PageModel class
        public int EmployeeID { get; set; }


        // Delete Employee
        public async Task<IActionResult> OnPostAsync()
        {
            if (EmployeeID <= 0)
            {
                _logger.LogWarning($"Cannot delete employee. Invalid employee ID: {EmployeeID}.");
                ModelState.AddModelError(string.Empty, "Please enter a valid employee ID.");
                return Page();
            }

            try
            {
                await _dataService.DeleteEmployeeAsync(EmployeeID);
                _logger.LogInformation($"Successfully deleted employee with ID: {EmployeeID}.");
                return RedirectToPage("/Employees");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred in DeleteEmployeeModel.");
                ModelState.AddModelError(string.Empty, "An error occurred while deleting the employee.");
                return Page();
            }
        }
    }
}
