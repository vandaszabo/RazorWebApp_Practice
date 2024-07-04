using Microsoft.AspNetCore.Mvc.RazorPages;
using MintaProjekt.Services;
using MintaProjekt.Models;
using Microsoft.AspNetCore.Mvc;

namespace MintaProjekt.Pages
{
    public class IndexModel : PageModel
    {
        private readonly EmployeeDataService _dataService;
        public IEnumerable<Employee> Employees { get; private set; }

        public IndexModel(EmployeeDataService dataService)
        {
            _dataService = dataService;
        }

        // Get all Employees from DB
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
    }
}
