using MintaProjekt.Models;
using System.Data.SqlClient;

namespace MintaProjekt.Services
{
    public class DepartmentDataService
    {
        private readonly string? _connectionString;
        private readonly ILogger<DepartmentDataService> _logger;

        public DepartmentDataService(ILogger<DepartmentDataService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _connectionString = configuration.GetConnectionString("MSSQLConnection");
        }

        // Retrieve all Departments from Database
        //public async Task<IEnumerable<Department>> GetDepartmentsAsync()
        //{
        //    var departments = new List<Department>();

        //    using (var connection = new SqlConnection(_connectionString))
        //    {
        //        await connection.OpenAsync(); // Open database connection
        //        var command = new SqlCommand("SELECT department_id, department_name, leader_id FROM tbl_department", connection);

        //        try
        //        {
        //            using (var reader = await command.ExecuteReaderAsync())
        //            {
        //                while (await reader.ReadAsync())
        //                {
        //                    departments.Add(new Department(
        //                        reader.GetInt32(0), // DepartmentID
        //                        reader.GetString(1), // DepartmentName
        //                        reader.IsDBNull(2) ? null : reader.GetInt32(2) // LeaderID
        //                    ));
        //                }
        //            }

        //        }
        //        catch (SqlException ex)
        //        {
        //            _logger.LogError(ex, "SQL Exception occurred while accessing SQL Server.");
        //            throw new ApplicationException("Error occurred while accessing SQL Server.", ex);
        //        }
        //        catch (Exception ex)
        //        {
        //            _logger.LogError(ex, "Exception occurred in DepartmentDataService.");
        //            throw new ApplicationException("Error occurred in DepartmentDataService.", ex);
        //        }

        //    }
        //    return departments;
        //}


        // Get all Department
        public async Task<IEnumerable<Department>> GetDepartmentsWithEmployeesAsync()
        {
            var departments = new Dictionary<int, Department>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(
                    "SELECT d.department_id, d.department_name, d.leader_id, e.employee_id, e.first_name, e.last_name " +
                    "FROM tbl_department d " +
                    "LEFT JOIN tbl_employee e ON d.department_id = e.department_id", connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            int departmentID = reader.GetInt32(0);
                            string departmentName = reader.GetString(1);
                            int? leaderID = reader.IsDBNull(2) ? (int?)null : reader.GetInt32(2);

                            if (!departments.TryGetValue(departmentID, out var department))
                            {
                                department = new Department(departmentID, departmentName, leaderID);
                                departments.Add(departmentID, department);
                            }

                            if (!reader.IsDBNull(3)) // Check if employee_id is not null
                            {
                                int employeeID = reader.GetInt32(3);
                                string firstName = reader.GetString(4);
                                string lastName = reader.GetString(5);
                                department.Employees.Add(new Employee
                                {
                                    EmployeeID = employeeID,
                                    FirstName = firstName,
                                    LastName = lastName,
                                    DepartmentID = departmentID
                                });
                            }
                        }
                    }
                }
            }

            return departments.Values;
        }

        public async Task UpdateDepartmentLeaderAsync(int NewLeaderID)
        {

        }

    }
}
