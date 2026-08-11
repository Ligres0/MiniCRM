using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;


namespace MiniCRM.Repositories
{
    public class AuthorizationRepository : IAuthorizationRepository
    {
        private readonly string _connectionString;

        public AuthorizationRepository(IConfiguration configuration)
        {
            _connectionString =
                configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "DefaultConnection bağlantısı bulunamadı. ");
        }

        private IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public bool HasPermission (int userId, string permissionCode)
        {
            using var connection = CreateConnection();

            const string sql = """
                SELECT COUNT(*)
                FROM UserRoles ur 
                INNER JOIN RolePermissions rp
                    ON ur.RoleId = rp.RoleId
                INNER JOIN Permissions p 
                    ON rp.PermissionId = p.Id
                WHERE ur.UserId = @UserId
                    AND p.Code = @PermissionCode;
                """;

            int count = connection.ExecuteScalar<int>(
                sql,
                new
                {
                    UserId = userId,
                    PermissionCode = permissionCode
                });
            return count > 0;
        }
        public List<string> GetUserPermissions(int userId)
        {
            using var connection = CreateConnection();

            const string sql = """
                SELECT DISTINCT p.Code
                FROM UserRoles ur
                INNER JOIN RolePermissions rp
                    ON ur.RoleId = rp.RoleId
                INNER JOIN Permissions p 
                    ON rp.PermissionId = p.Id
                WHERE ur.UserId =@UserId;
                """;

            return connection.Query<string>(sql, new {UserId = userId}).ToList();
        }
    }
}
