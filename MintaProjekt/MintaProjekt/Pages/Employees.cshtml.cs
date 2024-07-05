using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MintaProjekt.Models;
using MintaProjekt.Services;

namespace MintaProjekt.Pages
{
    public class EmployeesModel : PageModel
    {
        private readonly EmployeeDataService _dataService;
        public IEnumerable<Employee>? Employees { get; private set; }

        public EmployeesModel(EmployeeDataService dataService)
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
