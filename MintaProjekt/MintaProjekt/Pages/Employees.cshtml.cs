using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MintaProjekt.Exeptions;
using MintaProjekt.Models;
using MintaProjekt.Services;
using System.Data.SqlClient;

namespace MintaProjekt.Pages
{
    [Authorize(Policy = "CanSelectData")]
    public class EmployeesModel : PageModel
    {
        private readonly ILogger<EmployeesModel> _logger;
        private readonly IEmployeeDataService _dataService;
        public IEnumerable<Employee>? Employees { get; private set; }

        public EmployeesModel(ILogger<EmployeesModel> logger, IEmployeeDataService dataService)
        {
            _logger = logger;
            _dataService = dataService;
        }

        // Get all Employees from DB
        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                Employees = await _dataService.GetEmployeesAsync();
                return Page();
            }
            catch (SqlException)
            {
                _logger.LogError("Database related error occurred while retrieving employees.");
                return Page();
            }
            catch (NoRowsAffectedException)
            {
                _logger.LogError("There are no employees in the database.");
                return Page();
            }
            catch (Exception)
            {
                _logger.LogError("An error occurred while retrieving employees.");
                return Page();
            }
        }
    }
}
