using Dapper;
using Microsoft.Data.SqlClient;
using MiniCRM.Models;
using MiniCRM.ViewModels;
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
        public bool UsernameExists(string username)
        {
            using var connection = CreateConnection();

            const string sql = """
                SELECT COUNT(1)
                FROM Users
                WHERE UserName = @UserName;
                """;

            int count = connection.ExecuteScalar<int>(
                sql,
                new
                {
                    UserName = username
                });

            return count > 0;
        }

        public int Insert(User user)
        {
            using var connection = CreateConnection();

            const string sql = """
                INSERT INTO Users
                (
                    UserName,
                    PasswordHash,
                    Email,
                    IsActive,
                    CreatedDate
                )
                VALUES
                (
                    @UserName,
                    @PasswordHash,
                    @Email,
                    @IsActive,
                    @CreatedDate
                );
                SELECT CAST(SCOPE_IDENTITY() AS INT);
                """;

            return connection.QuerySingle<int>(sql, user);
        }

        public List<User> GetAll()
        {
            using var connection = CreateConnection ();
            const string sql = """
                SELECT
                    Id,
                    UserName,
                    Email,
                    IsActive,
                    CreatedDate
                FROM Users
                ORDER BY CreatedDate DESC;
                """;

            return connection.Query<User>(sql).ToList();
        }
        public List<UserListViewModel> GetAllWithRoles()
        {
            using var connection = CreateConnection();

            const string sql = """
        SELECT
            u.Id,
            u.UserName,
            u.Email,
            u.IsActive,
            u.CreatedDate,
            STRING_AGG(r.Name, ', ') AS RoleNames
        FROM Users u

        LEFT JOIN UserRoles ur
            ON u.Id = ur.UserId

        LEFT JOIN Roles r
            ON ur.RoleId = r.Id

        GROUP BY
            u.Id,
            u.UserName,
            u.Email,
            u.IsActive,
            u.CreatedDate

        ORDER BY
            u.CreatedDate DESC;
        """;

            return connection
                .Query<UserListViewModel>(sql)
                .ToList();
        }
        public List<Role> GetAllRoles()
        {
            using var connection =
                CreateConnection();

            const string sql = """
        SELECT
            Id,
            Name
        FROM Roles
        ORDER BY Name;
        """;

            return connection
                .Query<Role>(sql)
                .ToList();
        }
        public List<int> GetUserRoleIds(
    int userId)
        {
            using var connection =
                CreateConnection();

            const string sql = """
        SELECT RoleId
        FROM UserRoles
        WHERE UserId = @UserId;
        """;

            return connection
                .Query<int>(
                    sql,
                    new
                    {
                        UserId = userId
                    })
                .ToList();
        }
        public void UpdateUserRoles(
    int userId,
    List<int> roleIds)
        {
            using var connection = CreateConnection();

            connection.Open();

            using var transaction =
                connection.BeginTransaction();

            try
            {
                const string deleteSql = """
            DELETE FROM UserRoles
            WHERE UserId = @UserId;
            """;

                connection.Execute(
                    deleteSql,
                    new
                    {
                        UserId = userId
                    },
                    transaction);


                const string insertSql = """
            INSERT INTO UserRoles
            (
                UserId,
                RoleId
            )
            VALUES
            (
                @UserId,
                @RoleId
            );
            """;


                foreach (int roleId in roleIds)
                {
                    connection.Execute(
                        insertSql,
                        new
                        {
                            UserId = userId,
                            RoleId = roleId
                        },
                        transaction);
                }


                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();

                throw;
            }
        }
        public bool EmailExists(string email)
        {
            using var connection = CreateConnection();

            const string sql = """
        SELECT COUNT(1)
        FROM Users
        WHERE Email = @Email;
        """;

            int count = connection.ExecuteScalar<int>(
                sql,
                new
                {
                    Email = email
                });

            return count > 0;
        }
        public bool HasAnyRole(int userId)
        {
            using var connection = CreateConnection();

            const string sql = """
        SELECT COUNT(1)
        FROM UserRoles
        WHERE UserId = @UserId;
        """;

            int count = connection.ExecuteScalar<int>(
                sql,
                new
                {
                    UserId = userId
                });

            return count > 0;
        }
    }
}