using MintaProjekt.Enums;
using MintaProjekt.Exeptions;
using MintaProjekt.Models;
using System.Data.SqlClient;

namespace MintaProjekt.Services.Departments
{
    public class DepartmentDataService : IDepartmentDataService
    {
        private readonly string? _connectionString;
        private readonly ILogger<IDepartmentDataService> _logger;

        public DepartmentDataService(ILogger<IDepartmentDataService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _connectionString = configuration.GetConnectionString("MSSQLConnection");
        }


        public async Task<IEnumerable<Department>> GetDepartmentsWithEmployeesAsync()
        {
            _logger.LogInformation("Starting GetDepartmentsWithEmployeesAsync");

            try
            {
                // Create SQL Connection
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                // Create SQL Command
                string commandText = "EXEC sp_get_departments_with_employees_and_leaders";
                using var command = new SqlCommand(commandText, connection);

                // Execute
                _logger.LogInformation("Executing sp_get_departments_with_employees_and_leaders stored procedure.");
                using var reader = await command.ExecuteReaderAsync();

                // Log the schema information
                _logger.LogDebug("Columns returned by the stored procedure:");
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    _logger.LogDebug($"Column {i}: {reader.GetName(i)} ({reader.GetFieldType(i)})");
                }

                var departments = new Dictionary<int, Department>();
                while (await reader.ReadAsync())
                {
                    int departmentID = reader.GetInt32(0); // department_id
                    DepartmentName departmentName = Enum.Parse<DepartmentName>(reader.GetString(1)); // department_name
                    int? employeeID = reader.IsDBNull(2) ? null : reader.GetInt32(2); // employee_id
                    string? firstName = reader.IsDBNull(3) ? null : reader.GetString(3); // first_name
                    string? lastName = reader.IsDBNull(4) ? null : reader.GetString(4); // last_name
                    string? jobTitle = reader.IsDBNull(5) ? null : reader.GetString(5); // job_title
                    DateTime? startDate = reader.IsDBNull(6) ? null : reader.GetDateTime(6);  // (leadership) start_date
                    DateTime? endDate = reader.IsDBNull(7) ? null : reader.GetDateTime(7);    // (leadership) end_date

                    // Create department if not exist in dictionary
                    if (!departments.TryGetValue(departmentID, out var department))
                    {
                        department = new Department(departmentID, departmentName, new List<Employee>(), new List<Employee>());
                        departments.Add(departmentID, department);
                    }

                    // Add Employees and Leaders to Department if they exist
                    if (employeeID.HasValue)
                    {
                        var employee = new Employee
                        {
                            EmployeeID = employeeID.Value,
                            FirstName = firstName,
                            LastName = lastName,
                            JobTitle = jobTitle
                        };

                        department.Employees.Add(employee);

                        if (startDate.HasValue && (!endDate.HasValue || endDate > DateTime.Today))
                        {
                            department.Leaders.Add(employee);
                        }
                    }
                }

                // Validate the result
                if (departments.Values.Count == 0)
                {
                    _logger.LogWarning("No departments found.");
                    throw new NoRowsAffectedException("No departments found in the database.");
                }

                _logger.LogInformation("Departments retrieving process completed.");
                return departments.Values;

            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL Exception occurred in DepartmentDataService - GetDepartmentsWithEmployeesAsync method.");
                throw;
            }
            catch (NoRowsAffectedException ex)
            {
                _logger.LogError(ex, "NoRowsAffectedException occurred in DepartmentDataService - GetDepartmentsWithEmployeesAsync method.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred in DepartmentDataService - GetDepartmentsWithEmployeesAsync method.");
                throw;
            }
        }



        // Create department leader (Create new leader entity)
        public async Task AddDepartmentLeaderAsync(int departmentID, int newLeaderID)  // Leadership start date should changed to DateTime avoiding primary key duplication when updating in same day
        {
            _logger.LogInformation("Starting AddDepartmentLeaderAsync");

            try
            {
                // Create SQL Connection
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                // Create SQL Command
                string commandText = "EXEC sp_add_department_leader @EmployeeID, @DepartmentID, @StartDate";
                using SqlCommand command = new(commandText, connection);
                command.Parameters.AddWithValue("@EmployeeID", newLeaderID);
                command.Parameters.AddWithValue("@DepartmentID", departmentID);
                command.Parameters.AddWithValue("@StartDate", DateTime.Now);

                // Execute
                _logger.LogInformation("Executing sp_add_department_leader stored procedure.");
                int rowsAffected = await command.ExecuteNonQueryAsync();

                // Validate the result
                if (rowsAffected == 0)
                {
                    _logger.LogWarning("No rows were affected when adding the leader. Leader data: {EmployeeID}", newLeaderID);
                    throw new NoRowsAffectedException("No employee found with the provided ID for leader create.");
                }
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL Exception occurred in DepartmentDataService - AddDepartmentLeaderAsync method.");
                throw;
            }
            catch (NoRowsAffectedException ex)
            {
                _logger.LogError(ex, "NoRowsAffectedException occurred in DepartmentDataService - AddDepartmentLeaderAsync method");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while adding a department leader in DepartmentDataService - AddDepartmentLeaderAsync.");
                throw;
            }

        }

        // Delete department leader
        public async Task DeleteDepartmentLeaderAsync(int departmentID, int leaderID)
        {
            _logger.LogInformation("Starting DeleteDepartmentLeaderAsync");

            try
            {
                // Create SQL Connection
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                // Create SQL Command
                string commandText = "EXEC sp_delete_department_leader @EmployeeID, @DepartmentID, @EndDate";
                using SqlCommand command = new(commandText, connection);
                command.Parameters.AddWithValue("@EmployeeID", leaderID);
                command.Parameters.AddWithValue("@DepartmentID", departmentID);
                command.Parameters.AddWithValue("@EndDate", DateTime.Now);

                // Execute
                _logger.LogInformation("Executing sp_delete_department_leader stored procedure.");
                int rowsAffected = await command.ExecuteNonQueryAsync();

                // Validate the result
                if (rowsAffected == 0)
                {
                    _logger.LogWarning("No rows were affected when deleting the leader. Leader data: {EmployeeID}", leaderID);
                    throw new NoRowsAffectedException("No employee found with the provided ID for leader deletion.");
                }
                _logger.LogInformation("Delete department leader succeded.");
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL Exception occurred in DepartmentDataService - DeleteDepartmentLeaderAsync method.");
                throw;
            }
            catch (NoRowsAffectedException ex)
            {
                _logger.LogError(ex, "NoRowsAffectedException occurred in DepartmentDataService - DeleteDepartmentLeaderAsync method");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while deleting a department leader in DepartmentDataService - DeleteDepartmentLeaderAsync.");
                throw;
            }

        }

    }
}
