using MintaProjekt.Models;

namespace MintaProjekt.Services
{
    public interface IDepartmentDataService
    {
        Task<IEnumerable<Department>> GetDepartmentsWithEmployeesAsync();
        Task AddDepartmentLeaderAsync(int departmentID, int newLeaderID);
    }
}
