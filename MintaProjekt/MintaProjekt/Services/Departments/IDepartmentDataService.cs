using MintaProjekt.Models;

namespace MintaProjekt.Services.Departments
{
    public interface IDepartmentDataService
    {
        Task<IEnumerable<Department>> GetDepartmentsWithEmployeesAsync();
        Task AddDepartmentLeaderAsync(int departmentID, int newLeaderID);
        Task DeleteDepartmentLeaderAsync(int departmentID, int leaderID);
    }
}
