using Dapper;
using Microsoft.Data.SqlClient;
using MiniCRM.Models;
using MiniCRM.ViewModels;
using System.Data;

namespace MiniCRM.Repositories
{
    public class UserActivityLogRepository : IUserActivityLogRepository
    {
        private readonly string _connectionString;

        public UserActivityLogRepository(
            IConfiguration configuration)
        {
            _connectionString =
                configuration.GetConnectionString(
                    "DefaultConnection")
                ?? throw new InvalidOperationException(
                    "DefaultConnection bulunamadı.");
        }

        private IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public void Insert(UserActivityLog log)
        {
            using var connection = CreateConnection();

            const string sql = """
                INSERT INTO UserActivityLogs
                (
                    UserId,
                    Action,
                    Description,
                    CreatedDate
                )
                VALUES
                (
                    @UserId,
                    @Action,
                    @Description,
                    @CreatedDate
                );
                """;

            connection.Execute(sql, log);
        }
        public List<UserActivityLogListViewModel> GetAll()
        {
            using var connection = CreateConnection();

            const string sql = """
        SELECT
            l.Id,
            l.UserId,
            ISNULL(u.UserName, 'Bilinmeyen') AS UserName,
            l.Action,
            l.Description,
            l.CreatedDate
        FROM UserActivityLogs l

        LEFT JOIN Users u
            ON l.UserId = u.Id

        ORDER BY l.CreatedDate DESC;
        """;

            return connection
                .Query<UserActivityLogListViewModel>(sql)
                .ToList();
        }

    }
}
