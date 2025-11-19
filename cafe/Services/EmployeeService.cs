using System.Collections.Generic;
using Npgsql;
using RestaurantApp.Models;
using System.Threading.Tasks;

namespace RestaurantApp.Services
{
    public class EmployeeService
    {
        private readonly string _connectionString;

        public EmployeeService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<Employee>> GetAllEmployeesAsync()
        {
            var employees = new List<Employee>();
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand("SELECT * FROM employees WHERE is_active=true;", conn))
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            employees.Add(new Employee
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                FirstName = reader.GetString(reader.GetOrdinal("first_name")),
                                LastName = reader.GetString(reader.GetOrdinal("last_name")),
                                Position = reader.GetString(reader.GetOrdinal("position")),
                                HireDate = reader.GetDateTime(reader.GetOrdinal("hire_date")),
                                IsActive = reader.GetBoolean(reader.GetOrdinal("is_active")),
                                Email = reader.GetString(reader.GetOrdinal("email")),
                                PhoneNumber = reader.GetString(reader.GetOrdinal("phone_number"))
                            });
                        }
                    }
                }
            }

            return employees;
        }
        
    }
}