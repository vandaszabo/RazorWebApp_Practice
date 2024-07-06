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


        // Get all Department with related employees
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

                try
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            int departmentID = reader.GetInt32(0);
                            string departmentName = reader.GetString(1);
                            int? leaderID = reader.IsDBNull(2) ? (int?)null : reader.GetInt32(2);

                            if (!departments.TryGetValue(departmentID, out var department))  // If department is not yet in Dictionary, create and add
                            {
                                department = new Department(departmentID, departmentName, leaderID);
                                departments.Add(departmentID, department);
                            }

                            if (!reader.IsDBNull(3)) // Check if employee_id is not null & Then create and add the employee to the department
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
                catch (SqlException ex)
                {
                    _logger.LogError(ex, "SQL Exception occurred while accessing SQL Server in DepartmentDataService - GetDepartmentsWithEmployeesAsync method.");
                    throw new ApplicationException("Error occurred while accessing SQL Server.", ex);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Exception occurred in DepartmentDataService - GetDepartmentsWithEmployeesAsync method.");
                    throw new ApplicationException("Error occurred in DepartmentDataService.", ex);
                }
            }

            return departments.Values;
        }


        // Update department leader (can be null)
        public async Task UpdateDepartmentLeaderAsync(int departmentID, int? newLeaderID)
        {
            using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string query = "UPDATE tbl_department SET leader_id = @LeaderID WHERE department_id = @DepartmentID";
                    try
                    {
                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            if (newLeaderID.HasValue)
                            {
                                command.Parameters.AddWithValue("@LeaderID", newLeaderID.Value);
                            }
                            else
                            {
                                command.Parameters.AddWithValue("@LeaderID", DBNull.Value);
                            }

                            command.Parameters.AddWithValue("@DepartmentID", departmentID);

                            int rowsAffected = await command.ExecuteNonQueryAsync();

                            if (rowsAffected == 0)
                            {
                                _logger.LogWarning("No employee found with the provided ID for leader update.");
                                throw new Exception("No employee found with the provided ID for leader update.");
                            }
                        }
                    }

                    catch (SqlException sqlEx)
                    {
                        _logger.LogError(sqlEx, "SQL Exception occurred while accessing SQL Server in DepartmentDataService - UpdateDepartmentLeaderAsync method.");
                        throw new ApplicationException("Error occurred while accessing SQL Server.", sqlEx);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Exception occurred while updating a department leader.");
                        throw;
                    }
                }
            
        }

    }
}
