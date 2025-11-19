using System.Threading.Tasks;
using RestaurantApp.Models;
using Npgsql;
public class AuthenticationService
{
    private readonly string _connectionString;

    public AuthenticationService(string connectionString)
    {
        _connectionString = connectionString;
    }

    // Регистрация нового сотрудника
    public async Task RegisterAsync(Employee employee)
    {
        using (var conn = new NpgsqlConnection(_connectionString))
        {
            await conn.OpenAsync();
            var salt = BCrypt.GenerateSalt();
            var hash = BCrypt.HashPassword(employee.Password, salt);

            var insertCmd = new NpgsqlCommand(
                @"INSERT INTO employees(first_name, last_name, position, hire_date, email, phone_number, password_hash, password_salt, is_active) 
                  VALUES(@FirstName, @LastName, @Position, @HireDate, @Email, @PhoneNumber, @PasswordHash, @PasswordSalt, true)",
                conn
            );
            insertCmd.Parameters.AddWithValue("@FirstName", employee.FirstName);
            insertCmd.Parameters.AddWithValue("@LastName", employee.LastName);
            insertCmd.Parameters.AddWithValue("@Position", employee.Position);
            insertCmd.Parameters.AddWithValue("@HireDate", employee.HireDate);
            insertCmd.Parameters.AddWithValue("@Email", employee.Email);
            insertCmd.Parameters.AddWithValue("@PhoneNumber", employee.PhoneNumber);
            insertCmd.Parameters.AddWithValue("@PasswordHash", hash);
            insertCmd.Parameters.AddWithValue("@PasswordSalt", salt);
            await insertCmd.ExecuteNonQueryAsync();
        }
    }

    // Аутентификация сотрудника
    public async Task<bool> AuthenticateAsync(string email, string password)
    {
        using (var conn = new NpgsqlConnection(_connectionString))
        {
            await conn.OpenAsync();
            var selectCmd = new NpgsqlCommand("SELECT password_hash, password_salt FROM employees WHERE email=@Email AND is_active=true", conn);
            selectCmd.Parameters.AddWithValue("@Email", email);
            using (var reader = await selectCmd.ExecuteReaderAsync())
            {
                if (!await reader.ReadAsync()) return false;
                var storedHash = reader.GetString(reader.GetOrdinal("password_hash"));
                var salt = reader.GetString(reader.GetOrdinal("password_salt"));
                return BCrypt.Verify(password, storedHash);
            }
        }
    }
}