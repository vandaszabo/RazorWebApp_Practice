using Microsoft.AspNetCore.Mvc.RazorPages;
using MintaProjekt.Services;
using MintaProjekt.Models;
using Microsoft.AspNetCore.Mvc;

namespace MintaProjekt.Pages
{
    public class IndexModel : PageModel
    {
        private readonly DataService _dataService;
        public IEnumerable<Employee> Employees { get; private set; }

        public IndexModel(DataService dataService)
        {
            _dataService = dataService;
        }

        public async Task OnGetAsync()
        {
            try
            {
                Employees = await _dataService.GetEmployeesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while retrieving employees: {ex.Message}");
            }
        }

        public async Task<IActionResult> OnPostUpdateEmployeeAsync(Employee employee)
        {
            try
            {
                await _dataService.UpdateEmployeeAsync(employee);
                return RedirectToPage(); // Refresh the page to show the updated employee
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while updating an employee: {ex.Message}");
                return Page();
            }
        }
    }
}
