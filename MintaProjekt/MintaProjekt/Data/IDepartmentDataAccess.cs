using MintaProjekt.Models;

namespace MintaProjekt.Services
{
    public interface IDepartmentDataAccess
    {
        Task<IEnumerable<Department>> GetDepartmentsWithEmployeesAsync();
        Task AddDepartmentLeaderAsync(int departmentID, int newLeaderID);
        Task DeleteDepartmentLeaderAsync(int departmentID, int leaderID);
    }
}
