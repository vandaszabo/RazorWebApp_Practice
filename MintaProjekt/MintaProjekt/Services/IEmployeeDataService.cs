using MintaProjekt.Models;

namespace MintaProjekt.Services
{
    public interface IEmployeeDataService
    {
        Task<IEnumerable<Employee>> GetEmployeesAsync();
        Task<Employee> GetEmployeeByIDAsync(int employeeID);
        Task AddEmployeeAsync(Employee employee, int userID);
        Task UpdateEmployeeAsync(Employee employee, int userID);
        Task DeleteEmployeeAsync(int employeeID, int userID);
    }
}
