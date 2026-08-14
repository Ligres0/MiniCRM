using Dapper;
using Microsoft.Data.SqlClient;
using MiniCRM.Models;
using System.Data;

namespace MiniCRM.Repositories
{
    public class RolePermissionRepository: IRolePermissionRepository
    {
        private readonly string _connectionString;

        public RolePermissionRepository(IConfiguration configuration)
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
        public List<Permission> GetAllPermissions()
        {
            using var connection = CreateConnection();

            const string sql = """
                SELECT
                    Id,
                    Code
                FROM Permissions
                ORDER BY Code;
                """;

            return connection.Query<Permission>(sql).ToList();
        }
        public List<int> GetRolePermissionIds(int roleId)
        {
            using var connection = CreateConnection();

            const string sql = """
                SELECT PermissionId
                FROM RolePermissions
                WHERE RoleId = @RoleId;
                """;

            return connection.Query<int>(sql,
                new
                {
                    RoleId = roleId
                })
                .ToList();

        }

        public void UpdateRolePermission(int roleId, List<int> permissionIds)
        {
            using var connection = CreateConnection();

            connection.Open();

            using var transaction = connection.BeginTransaction();

            try
            {
                const string deleteSql = """
                    DELETE FROM RolePermissions
                    WHERE RoleId = @RoleId;
                    """;

                connection.Execute(deleteSql,
                    new
                    {
                        RoleId = roleId
                    }, transaction);

                const string insertSql = """
                    INSERT INTO RolePermissions
                    (
                        RoleId,
                        PermissionId
                    )
                    VALUES
                    (
                        @RoleId,
                        @PermissionId
                    );
                    """;

                foreach (int permissionId in permissionIds)
                {
                    connection.Execute(insertSql,
                        new
                        {
                            RoleId = roleId,
                            PermissionId = permissionId
                        }, transaction);
                }
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        public List<int> GetUserIdsByRoleId(int roleId)
        {
            using var connection = CreateConnection();

            const string sql = """
        SELECT UserId
        FROM UserRoles
        WHERE RoleId = @RoleId;
        """;

            return connection.Query<int>(
                sql,
                new
                {
                    RoleId = roleId
                })
                .ToList();
        }

        public bool RoleNameExists(string name)
        {
            using var connection = CreateConnection();
            const string sql = """
                SELECT COUNT(1)
                FROM Roles
                WHERE Name = @Name;
                """;

            int count = connection.ExecuteScalar<int>(sql,
                new
                {
                    Name = name
                });
            return count > 0;
        }
        
        public int InsertRole(string name)
        {
            using var connection = CreateConnection();
            const string sql = """
                INSERT INTO Roles
                (
                    Name
                )
                VALUES
                (
                    @Name
                );
                SELECT CAST(SCOPE_IDENTITY() AS INT);

                """;

            return connection.QuerySingle<int>(sql, new
            {
                Name = name
            });
        }
    }
}
