using Dapper;
using Microsoft.Data.SqlClient;
using MiniCRM.Models;
using System.Data;

namespace MiniCRM.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly string _connectionString;

        public CategoryRepository(IConfiguration configuration)
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

        public List<Category> GetAllCategories()
        {
            using var connection = CreateConnection();

            const string sql = """
                SELECT
                    Id,
                    Name,
                    IsActive
                FROM Categories
                ORDER BY Name
                """;

            return connection.Query<Category>(sql).ToList();
        }

        public List<Category> GetAllActive()
        {
            using var connection = CreateConnection();

            const string sql = """
                SELECT
                    Id,
                    Name,
                    IsActive
                FROM Categories
                WHERE IsActive = 1
                ORDER BY Name
                """;

            return connection.Query<Category>(sql).ToList();
        }
        public Category? GetById(int id)
        {
            using var connection = CreateConnection();

            const string sql = """
                SELECT
                    Id,
                    Name,
                    IsActive
                FROM Categories
                WHERE Id = @Id
                """;

            return connection.QueryFirstOrDefault<Category>(
                sql,
                new { Id = id });
        }
        public int Insert(Category category)
        {
            using var connection = CreateConnection();

            const string sql = """
                INSERT INTO Categories
                (
                    Name,
                    IsActive
                )
                VALUES
                (
                    @Name,
                    @IsActive
                );
                """;

            return connection.Execute(sql, category);
        }

        public int Update(Category category)
        {
            using var connection = CreateConnection();

            const string sql = """
                UPDATE Categories
                SET
                    Name = @Name,
                    IsActive = @IsActive
                WHERE Id = @Id;
                """;

            return connection.Execute(sql, category);
        }
        public int Deactivate(int id)
        {
            using var connection = CreateConnection();

            const string sql = """
                UPDATE Categories
                SET IsActive = 0
                WHERE Id = @Id
                  AND IsActive = 1;
                """;

            return connection.Execute(sql, new { Id = id });
        }
        public bool CategoryNameExists(
           string name,
           int? excludeId = null)
        {
            using var connection = CreateConnection();

            const string sql = """
                SELECT COUNT(*)
                FROM Categories
                WHERE Name = @Name
                  AND (@ExcludeId IS NULL OR Id <> @ExcludeId);
                """;

            int count = connection.ExecuteScalar<int>(
                sql,
                new
                {
                    Name = name,
                    ExcludeId = excludeId
                });

            return count > 0;
        }


    }
}