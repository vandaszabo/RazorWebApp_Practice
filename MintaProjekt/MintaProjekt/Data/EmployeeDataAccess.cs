using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using MintaProjekt.Models;
using MintaProjekt.Exeptions;
using MintaProjekt.Enums;
using Microsoft.AspNetCore.Authorization;

namespace MintaProjekt.Services
{
    public class EmployeeDataAccess : IEmployeeDataAccess
    {
        private readonly string? _connectionString;
        private readonly ILogger<IEmployeeDataAccess> _logger;

        public EmployeeDataAccess(ILogger<IEmployeeDataAccess> logger, IConfiguration configuration)
        {
            _logger = logger;
            _connectionString = configuration.GetConnectionString("MSSQLConnection");
        }


        // Retrieve all Employees from Database
        public async Task<IEnumerable<Employee>> GetEmployeesAsync()
        {
            _logger.LogInformation("Starting GetEmployeeAsync");

                try 
                { 
                    // Create SQL Connection
                    using var connection = new SqlConnection(_connectionString);
                    await connection.OpenAsync();

                    // Create SQL Command
                    string commandText = "EXEC sp_get_employees";
                    var command = new SqlCommand(commandText, connection);

                    // Execute
                    _logger.LogInformation("Executing sp_get_employees stored procedure.");
                    using var reader = await command.ExecuteReaderAsync();

                    var employees = new List<Employee>();
                    while (await reader.ReadAsync())
                    {
                        employees.Add(new Employee(
                            reader.GetInt32(0),  // EmployeeID
                            reader.GetString(1), // FirstName
                            reader.GetString(2), // LastName
                            reader.GetString(3), // Email
                            PhoneNumber.Parse(reader.GetString(4)), // PhoneNumber
                            DateOnly.FromDateTime(reader.GetDateTime(5)), // HireDate
                            reader.GetString(6), // JobTitle
                            Enum.Parse<DepartmentName>(reader.GetString(7)) // DepartmentName
                        ));
                    }

                    if(employees.Count == 0)
                    {
                        _logger.LogWarning("No employees found.");
                        throw new NoRowsAffectedException("No employees found in the database.");
                    }

                    return employees;

                }
                catch (SqlException ex)
                {
                    _logger.LogError(ex, "SQL Exception occurred in EmployeeDataService - GetEmployeesAsync method.");
                    throw;
                }
                catch (NoRowsAffectedException ex)
                {
                    _logger.LogError(ex, "NoRowsAffectedException occurred in EmployeeDataService - GetEmployeesAsync method.");
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Exception occurred in EmployeeDataService - GetEmployeesAsync method.");
                    throw;
                }

        }


        // Find Employee by ID
        public async Task<Employee> GetEmployeeByIDAsync(int employeeID)
        {
            _logger.LogInformation("Start GetEmployeeByIDAsync method.");
            _logger.LogDebug("Received ID must be greater than zero.");
            _logger.LogInformation("Received employee ID: {ID}", employeeID);

            // Validate employeeID
            if (employeeID <= 0)
            {
                _logger.LogWarning("Invalid employee ID: {ID}", employeeID);
                throw new ArgumentException("Invalid employee ID. ID must be greater than zero.");
            }

            try
            {
                // Create SQL Connection
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                // Create SQL Command
                string commandText = "EXEC sp_get_employee_by_id @EmployeeID";
                using SqlCommand command = new(commandText, connection);
                command.Parameters.Add(new SqlParameter("@EmployeeID", employeeID));

                // Execute
                _logger.LogInformation("Executing sp_get_employee_by_id stored procedure.");
                using SqlDataReader reader = await command.ExecuteReaderAsync();
                
                Employee employee = new();
                if (await reader.ReadAsync())
                {
                    employee = new Employee(
                            reader.GetInt32(0),  // EmployeeID
                            reader.GetString(1), // FirstName
                            reader.GetString(2), // LastName
                            reader.GetString(3), // Email
                            PhoneNumber.Parse(reader.GetString(4)), // PhoneNumber
                            DateOnly.FromDateTime(reader.GetDateTime(5)), // HireDate
                            reader.GetString(6), // JobTitle
                            Enum.Parse<DepartmentName>(reader.GetString(7)) // DepartmentName
                    );
                    if (employee.HasInvalidProperties())
                    {
                        _logger.LogWarning("Employee with ID {EmployeeID} has invalid properties.", employeeID);
                        throw new Exception($"Employee with ID {employeeID} has invalid properties.");
                    }

                    return employee;
                }
                else
                {
                    _logger.LogError("No employee found with ID {EmployeeID}.", employeeID);
                    throw new KeyNotFoundException($"No employee found with ID {employeeID}.");
                }
            }
            catch(KeyNotFoundException ex)
            {
                _logger.LogError(ex, "KeyNotFoundException occurred in EmployeeDataService - GetEmployeeByIDAsync method.");
                throw;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL Exception occurred in EmployeeDataService - GetEmployeeByIDAsync method.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while reading an employee.");
                throw;
            }
        }


        // Create new Employee
        public async Task AddEmployeeAsync(Employee employee, string userID)
        {
            _logger.LogInformation("Start AddEmployeeAsync method.");
            _logger.LogDebug("Received ID must be greater than zero.");
            _logger.LogInformation("Received employee: {Employee}", employee.ToString());

            // Validate Employee object
            if (employee.HasInvalidProperties())
            {
                _logger.LogWarning("Invalid Employee object passed to AddEmployeeAsync. It has null or invalid property.");
                throw new ArgumentException("Invalid Employee: All fields must be provided.");
            }

            try
            {
                // Create SQL Connection
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                // Create SQL Command
                string commandText = "EXEC sp_add_employee @FirstName, @LastName, @Email, @PhoneNumber, @HireDate, @JobTitle, @DepartmentID, @CreatedAt, @CreatedBy";
                using SqlCommand command = new(commandText, connection);
                command.Parameters.Add(new SqlParameter("@FirstName", employee.FirstName));
                command.Parameters.Add(new SqlParameter("@LastName", employee.LastName));
                command.Parameters.Add(new SqlParameter("@Email", employee.Email));
                command.Parameters.Add(new SqlParameter("@PhoneNumber", employee.PhoneNumber.ToString()));
                command.Parameters.Add(new SqlParameter("@HireDate", employee.HireDate.ToDateTime(TimeOnly.MinValue)));
                command.Parameters.Add(new SqlParameter("@JobTitle", employee.JobTitle));
                command.Parameters.Add(new SqlParameter("@DepartmentID", (int)employee.DepartmentName));
                command.Parameters.Add(new SqlParameter("@CreatedAt", DateTime.Now));
                command.Parameters.Add(new SqlParameter("@CreatedBy", userID));

                // Execute
                _logger.LogInformation("Executing sp_add_employee stored procedure.");
                int rowsAffected = await command.ExecuteNonQueryAsync();

                // Validate the result
                if (rowsAffected == 0)
                {
                    _logger.LogWarning("No rows were affected when adding the employee. Employee data: {Employee}", employee.ToString());
                    throw new NoRowsAffectedException("Failed to add the employee to the database."); // Warning nem error
                }

                _logger.LogInformation("Employee added successfully. Rows affected: {RowsAffected}", rowsAffected);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "ArgumentExeption occured in EmployeeDataService - AddEmployeeAsync method.");
                throw ; // UI Exeption (többnyelvű)
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL Exception occurred while accessing SQL Server in EmployeeDataService - AddEmployeeAsync method.");
                throw;
            }
            catch (NoRowsAffectedException ex)
            {
                _logger.LogError(ex, "NoRowsAffectedException occurred in EmployeeDataService - AddEmployeeAsync method.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while adding an employee.");
                throw;
            }
        }


        // Update existing Employee
        public async Task UpdateEmployeeAsync(Employee employee, string userID)
        {
            _logger.LogDebug("All properties of the Employee object must be non-null.");
            _logger.LogInformation("Received Employee object for update {Employee}", employee.ToString());

            // Validate Employee object
            if (employee.HasInvalidProperties())
            {
                _logger.LogWarning("Invalid Employee object passed to UpdateEmployeeAsync. It has null or invalid property.");
                throw new ArgumentException("Invalid Employee data. All properties must be provided.");
            }

            try
            {
                // Create SQL connection
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                // Create SQL Command
                string commandText = $"EXEC sp_set_employee @EmployeeID, @FirstName, @LastName, @Email, @PhoneNumber, @HireDate, @JobTitle, @DepartmentID, @ModifiedAt, @ModifiedBy";
                using SqlCommand command = new(commandText, connection);
                command.Parameters.Add(new SqlParameter("@FirstName", employee.FirstName));
                command.Parameters.Add(new SqlParameter("@LastName", employee.LastName));
                command.Parameters.Add(new SqlParameter("@Email", employee.Email));
                command.Parameters.Add(new SqlParameter("@PhoneNumber", employee.PhoneNumber.ToString()));
                command.Parameters.Add(new SqlParameter("@HireDate", employee.HireDate.ToDateTime(TimeOnly.MinValue)));
                command.Parameters.Add(new SqlParameter("@JobTitle", employee.JobTitle));
                command.Parameters.Add(new SqlParameter("@DepartmentID", (int)employee.DepartmentName));
                command.Parameters.Add(new SqlParameter("@ModifiedAt", DateTime.Now));
                command.Parameters.Add(new SqlParameter("@ModifiedBy", userID));

                // Execute
                _logger.LogInformation("Executing sp_set_employee stored procedure.");
                int rowsAffected = await command.ExecuteNonQueryAsync();

                // Validate the result
                if (rowsAffected == 0)
                {
                    _logger.LogWarning("No employee found with the provided ID to update.");
                    throw new NoRowsAffectedException("The SQL query affected zero rows. Unseccessful employee update.");
                }

                _logger.LogInformation("Employee updated successfully. Rows affected: {RowsAffected}", rowsAffected);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Invalid Employee object provided for update.");
                throw;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL Exception occurred in EmployeeDataService - UpdateEmployeeAsync method.");
                throw;
            }
            catch (NoRowsAffectedException ex)
            {
                _logger.LogError(ex, "NoRowsAffectedException occurred in EmployeeDataService - UpdateEmployeeAsync method. Employee update failed.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while updating an employee.");
                throw;
            }
        }


        // Delete Employee by ID
        public async Task DeleteEmployeeAsync(int employeeID, string userID)
        {
            _logger.LogInformation("Start DeleteEmployeeAsync method.");
            _logger.LogDebug("Received ID must be greater than zero.");
            _logger.LogInformation("Received employee ID: {ID}", employeeID);

            // Validate employeeID
            if (employeeID <= 0)
            {
                _logger.LogWarning("Invalid employee ID for deletion: {ID}", employeeID);
                throw new ArgumentException("Invalid employee ID. ID must be greater than zero.");
            }

            try
            {
                // Create SQL connection
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                // Create SQL Command
                string commandText = "EXEC sp_delete_employee @EmployeeID, @DeletedAt, @DeletedBy";
                using SqlCommand command = new(commandText, connection);
                command.Parameters.Add(new SqlParameter("@EmployeeID", employeeID));
                command.Parameters.Add(new SqlParameter("@DeletedAt", DateTime.Now));
                command.Parameters.Add(new SqlParameter("@DeletedBy", userID));

                // Execute 
                _logger.LogInformation("Executing sp_delete_employee stored procedure.");
                int rowsAffected = await command.ExecuteNonQueryAsync();

                // Validate the result
                if (rowsAffected == 0)
                {
                    _logger.LogWarning("No employee found with the provided ID to delete.");
                    throw new NoRowsAffectedException("The SQL query affected zero rows. Unseccessful employee deletion.");
                }

                _logger.LogInformation("Employee deleted successfully. Rows affected: {RowsAffected}", rowsAffected);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL Exception occurred in EmployeeDataService - DeleteEmployeeAsync method.");
                throw;
            }
            catch (NoRowsAffectedException ex)
            {
                _logger.LogError(ex, "NoRowsAffectedException occurred in EmployeeDataService - DeleteEmployeeAsync method. Employee deletion failed.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred in EmployeeDataService - DeleteEmployeeAsync method.");
                throw;
            }
        }

    }
}
