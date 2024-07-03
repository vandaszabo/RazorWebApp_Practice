using System.Data.SqlClient;
using MintaProjekt.Models;

namespace MintaProjekt.Services
{
    public class DataService
    {
        private readonly string? _connectionString;
        private readonly ILogger<DataService> _logger;

        public DataService(ILogger<DataService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _connectionString = configuration.GetConnectionString("MSSQLConnection");
        }

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
                                DateOnly.FromDateTime(reader.GetDateTime(5)), // HireDate
                                reader.GetString(6), // JobTitle
                                reader.GetInt32(7)  // DepartmentID
                            ));
                        }
                    }

                }
                catch (SqlException ex)
                {
                    _logger.LogError(ex, "SQL Exception occurred while accessing SQL Server.");
                    throw new ApplicationException("Error occurred while accessing SQL Server.", ex);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Exception occurred in DataService.");
                    throw new ApplicationException("Error occurred in DataService.", ex);
                }

            }
            return employees;
        }

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
                        command.Parameters.Add(new SqlParameter("@HireDate", employee.HireDate.ToDateTime(TimeOnly.MinValue))); // Convert DateOnly to DateTime
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
