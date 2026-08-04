using Dapper;
using Microsoft.Data.SqlClient;
using MiniCRM.Models;
using System.Data;


namespace MiniCRM.Repositories
{
    public class ProductRepository: IProductRepository
    {
        private readonly string _connectionString;

        private IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public ProductRepository(IConfiguration configuration)
        {
            _connectionString =
                configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "DefaultConnection bağlantısı bulunamadı.");
        }

        public Product? GetById(int id)
        {
            using var connection = CreateConnection();

            const string sql = """
                SELECT
                    p.Id,
                    p.Name,
                    p.CategoryId,
                    p.Price,
                    p.Stock,
                    p.IsActive,
                    p.CreatedDate,
                    c.Name AS CategoryName
                FROM Products p
                INNER JOIN Categories c
                    ON p.CategoryId = c.Id
                WHERE p.Id = @Id;
                """;

            return connection.QueryFirstOrDefault<Product>(
                sql,
                new { Id = id });
        }

        public int Insert(Product product)
        {
            using var connection = CreateConnection();
            const string sql = """
                INSERT INTO Products
                (
                    Name,
                    CategoryId,
                    Price,
                    Stock,
                    IsActive
                )
                VALUES
                (
                    @Name,
                    @CategoryId,
                    @Price,
                    @Stock,
                    @IsActive
                );
            """;
            return connection.Execute(sql, product);
        }
        public int Update(Product product) {
            using var connection = CreateConnection();
            const string sql = """
                UPDATE Products
                SET
                    Name = @Name,
                    CategoryId = @CategoryId,
                    Price = @Price,
                    Stock = @Stock,
                    IsActive = @IsActive
                WHERE Id = @Id;
            """;
            return connection.Execute(sql, product);
        }
        public int Deactivate(int id)
        {
            using var connection = CreateConnection();
            const string sql = "UPDATE Products SET IsActive = 0 WHERE Id = @Id";
            return connection.Execute(sql, new { Id = id });
        }
        public bool NameExists(string name, int? excludeProductId = null)
        {
            using var connection = CreateConnection();
            const string sql = """
                SELECT COUNT(1) 
                FROM Products 
                WHERE Name = @Name 
                AND (@ExcludeProductId IS NULL OR Id <> @ExcludeProductId)
            """;
            return connection.ExecuteScalar<int>(sql, new { Name = name, ExcludeProductId = excludeProductId }) > 0;
        }
        public List<Product> GetFilteredPaged(string? search, int? categoryId, bool? isActive, int pageNumber, int pageSize)
        {
            using var connection = CreateConnection();
            var sql = """
                SELECT * FROM Products
                WHERE (@Search IS NULL OR Name LIKE '%' + @Search + '%')
                AND (@CategoryId IS NULL OR CategoryId = @CategoryId)
                AND (@IsActive IS NULL OR IsActive = @IsActive)
                ORDER BY Name
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
            """;
            return connection.Query<Product>(sql, new
            {
                Search = search,
                CategoryId = categoryId,
                IsActive = isActive,
                Offset = (pageNumber - 1) * pageSize,
                PageSize = pageSize
            }).ToList();
        }

        public int GetFilteredCount(string? search, int? categoryId, bool? isActive)
        {
            using var connection = CreateConnection();
            var sql = """
                SELECT COUNT(*) FROM Products
                WHERE (@Search IS NULL OR Name LIKE '%' + @Search + '%')
                AND (@CategoryId IS NULL OR CategoryId = @CategoryId)
                AND (@IsActive IS NULL OR IsActive = @IsActive);
            """;
            return connection.ExecuteScalar<int>(sql, new
            {
                Search = search,
                CategoryId = categoryId,
                IsActive = isActive
            });
        }
    }
}
