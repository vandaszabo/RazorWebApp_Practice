using MintaProjekt.Enums;
using MintaProjekt.Exeptions;
using MintaProjekt.Models;
using System.Data.SqlClient;

namespace MintaProjekt.Services
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


        // Get all Department with related employees
        public async Task<IEnumerable<Department>> GetDepartmentsWithEmployeesAsync()
        {
            _logger.LogInformation("Starting GetDepartmentsWithEmployeesAsync");

            try
            {
                // Create SQL Connection
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                // Create SQL Command
                string commandText = "EXEC sp_get_departments_with_employees";
                using var command = new SqlCommand(commandText, connection);

                // Execute
                _logger.LogInformation("Executing sp_get_departments_with_employees_and_leaders stored procedure.");
                using var reader = await command.ExecuteReaderAsync();

                var departments = new Dictionary<int, Department>();
                while (await reader.ReadAsync())
                {
                    int departmentID = reader.GetInt32(0);
                    DepartmentName departmentName = Enum.Parse<DepartmentName>(reader.GetString(1));
                    int employeeID = reader.GetInt32(2);
                    string firstName = reader.GetString(3);
                    string lastName = reader.GetString(4);
                    string jobTitle = reader.GetString(5);
                    DateTime? startDate = reader.IsDBNull(6) ? null : reader.GetDateTime(6);  // leadership start date
                    DateTime? endDate = reader.IsDBNull(7) ? null : reader.GetDateTime(7);    // leadership end date

                    // Create department if not exist in dictionary
                    if (!departments.TryGetValue(departmentID, out var department))
                    {
                        department = new Department(departmentID, departmentName, new List<Employee>(), new List<Employee>());
                        departments.Add(departmentID, department);
                    }

                    // Add Employees and Leaders to Department
                    department.Employees.Add(new Employee
                    {
                        EmployeeID = employeeID,
                        FirstName = firstName,
                        LastName = lastName,
                        JobTitle = jobTitle
                    });

                    if (startDate.HasValue && (!endDate.HasValue || endDate >= DateTime.Today))
                    {
                        department.Leaders.Add(new Employee
                        {
                            EmployeeID = employeeID,
                            FirstName = firstName,
                            LastName = lastName
                        });
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
            catch (NoRowsAffectedException ex)
            {
                _logger.LogError(ex, "NoRowsAffectedException occurred in DepartmentDataService - GetDepartmentsWithEmployeesAsync method.");
                throw;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL Exception occurred in DepartmentDataService - GetDepartmentsWithEmployeesAsync method.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred in DepartmentDataService - GetDepartmentsWithEmployeesAsync method.");
                throw;
            }   
        }


        // Create department leader (Create new leader entity)
        public async Task AddDepartmentLeaderAsync(int departmentID, int newLeaderID)
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
            catch (NoRowsAffectedException ex)
            {
                _logger.LogError(ex, "NoRowsAffectedException occurred in DepartmentDataService - AddDepartmentLeaderAsync method");
                throw;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL Exception occurred in DepartmentDataService - AddDepartmentLeaderAsync method.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while adding a department leader in DepartmentDataService - AddDepartmentLeaderAsync.");
                throw;
            }

        }

    }
}
