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
        public async Task<IEnumerable<Department>> GetDepartmentsAsync()
        {
            var departments = new List<Department>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(); // Open database connection
                var command = new SqlCommand("SELECT department_id, department_name, leader_id", connection);

                try
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            departments.Add(new Department(
                                reader.GetInt32(0), // DepartmentID
                                reader.GetString(1), // DepartmentName
                                reader.GetInt32(2)  // LeaderID
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
                    _logger.LogError(ex, "Exception occurred in DepartmentDataService.");
                    throw new ApplicationException("Error occurred in DepartmentDataService.", ex);
                }

            }
            return departments;
        }
    }
}
