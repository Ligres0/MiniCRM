using Dapper;
using Microsoft.Data.SqlClient;
using MiniCRM.Models;
using System.Data;

namespace MiniCRM.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(IConfiguration configuration)
        {
            _connectionString =
                configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "DefaultConnection bağlantısı bulunamadı.");
        }

        private IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public User? GetByUsername(string username)
        {
            using var connection = CreateConnection();

            const string sql = """
                SELECT
                    Id,
                    UserName,
                    PasswordHash,
                    Email,
                    IsActive,
                    CreatedDate
                FROM Users
                WHERE UserName = @UserName
                  AND IsActive = 1;
                """;

            return connection.QueryFirstOrDefault<User>(
                sql,
                new { UserName = username });
        }
    }
}