﻿using MintaProjekt.Models;

namespace MintaProjekt.Services.Employees
{
    public interface IEmployeeDataService
    {
        Task<IEnumerable<Employee>> GetEmployeesAsync();
        Task<IEnumerable<Employee>> GetEmployeesForPage(int recordSkip, int pageSize = 10);
        Task<int> GetEmployeesCount();
        Task<Employee> GetEmployeeByIDAsync(int employeeID);
        Task AddEmployeeAsync(Employee employee, string userID);
        Task UpdateEmployeeAsync(Employee employee, string userID);
        Task DeleteEmployeeAsync(int employeeID, string userID);
    }
}
