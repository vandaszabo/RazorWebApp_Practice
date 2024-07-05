using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using MintaProjekt.Models;

namespace MintaProjekt.Services
{
    public class EmployeeDataService
    {
        private readonly string? _connectionString;
        private readonly ILogger<EmployeeDataService> _logger;

        public EmployeeDataService(ILogger<EmployeeDataService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _connectionString = configuration.GetConnectionString("MSSQLConnection");
        }


        // Retrieve all Employees from Database
        public async Task<IEnumerable<Employee>> GetEmployeesAsync()
        {
            var employees = new List<Employee>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(); // Open database connection
                var command = new SqlCommand("SELECT employee_id, first_name, last_name, email, phone_number, hire_date, job_title, department_id FROM tbl_employee", connection);

                try
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            employees.Add(new Employee(
                                reader.GetInt32(0),  // EmployeeID
                                reader.GetString(1), // FirstName
                                reader.GetString(2), // LastName
                                reader.GetString(3), // Email
                                reader.GetString(4), // PhoneNumber
                                DateOnly.FromDateTime(reader.GetDateTime(5)), // HireDate - Parse it from DateTime to DateOnly
                                reader.GetString(6), // JobTitle
                                reader.GetInt32(7)  // DepartmentID
                            ));
                        }
                    }

                }
                catch (SqlException ex)
                {
                    _logger.LogError(ex, "SQL Exception occurred while accessing SQL Server in EmployeeDataService - GetEmployeesAsync method.");
                    throw new ApplicationException("Error occurred while accessing SQL Server.", ex);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Exception occurred in EmployeeDataService - GetEmployeesAsync method.");
                    throw new ApplicationException("Error occurred in EmployeeDataService.", ex);
                }

            }
            return employees;
        }


        // Find Employee by ID
        public async Task<Employee> GetEmployeeByIDAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT employee_id, first_name, last_name, email, phone_number, hire_date, job_title, department_id FROM tbl_employee WHERE employee_id = @EmployeeID";
                Employee? employee = null;

                try
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add(new SqlParameter("@EmployeeID", id));
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                employee = new Employee
                                {
                                    EmployeeID = reader.GetInt32(0),
                                    FirstName = reader.GetString(1),
                                    LastName = reader.GetString(2),
                                    Email = reader.GetString(3),
                                    PhoneNumber = reader.GetString(4),
                                    HireDate = DateOnly.FromDateTime(reader.GetDateTime(5)),
                                    JobTitle = reader.GetString(6),
                                    DepartmentID = reader.GetInt32(7)
                                };
                            }
                        }
                    }
                    if (employee == null)
                    {
                        _logger.LogError("No employee found with ID: {EmployeeID}", id);
                        throw new Exception($"No employee found with ID: {id}");
                    }
                    return employee;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Exception occurred while reading an employee.");
                    throw;
                }
            }
        }


        // Create new Employee
        public async Task AddEmployeeAsync(Employee employee)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "INSERT INTO tbl_employee (first_name, last_name, email, phone_number, hire_date, job_title, department_id) " +
                               "VALUES (@FirstName, @LastName, @Email, @PhoneNumber, @HireDate, @JobTitle, @DepartmentID)";

                try
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add(new SqlParameter("@FirstName", employee.FirstName));
                        command.Parameters.Add(new SqlParameter("@LastName", employee.LastName));
                        command.Parameters.Add(new SqlParameter("@Email", employee.Email));
                        command.Parameters.Add(new SqlParameter("@PhoneNumber", employee.PhoneNumber));
                        command.Parameters.Add(new SqlParameter("@HireDate", employee.HireDate.ToDateTime(TimeOnly.MinValue))); // Convert DateOnly to DateTime
                        command.Parameters.Add(new SqlParameter("@JobTitle", employee.JobTitle));
                        command.Parameters.Add(new SqlParameter("@DepartmentID", employee.DepartmentID));

                        await command.ExecuteNonQueryAsync();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Exception occurred while adding an employee.");
                    throw;
                }
            }
        }


        // Update existing Employee
        public async Task UpdateEmployeeAsync(Employee employee)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "UPDATE tbl_employee SET first_name = @FirstName, last_name = @LastName, email = @Email, " +
                               "phone_number = @PhoneNumber, hire_date = @HireDate, job_title = @JobTitle, department_id = @DepartmentID " +
                               "WHERE employee_id = @EmployeeID";
                try
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add(new SqlParameter("@EmployeeID", employee.EmployeeID));
                        command.Parameters.Add(new SqlParameter("@FirstName", employee.FirstName));
                        command.Parameters.Add(new SqlParameter("@LastName", employee.LastName));
                        command.Parameters.Add(new SqlParameter("@Email", employee.Email));
                        command.Parameters.Add(new SqlParameter("@PhoneNumber", employee.PhoneNumber));
                        command.Parameters.Add(new SqlParameter("@HireDate", employee.HireDate.ToDateTime(TimeOnly.MinValue)));
                        command.Parameters.Add(new SqlParameter("@JobTitle", employee.JobTitle));
                        command.Parameters.Add(new SqlParameter("@DepartmentID", employee.DepartmentID));

                        await command.ExecuteNonQueryAsync();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Exception occurred while updating an employee.");
                    throw;
                }
            }
        }


        // Delete Employee by ID
        public async Task DeleteEmployeeAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "DELETE FROM tbl_employee WHERE employee_id = @EmployeeID";

                try
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add(new SqlParameter("@EmployeeID", id));
                        await command.ExecuteNonQueryAsync();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Exception occurred while deleting an employee.");
                    throw;
                }
            }
        }

    }
}
