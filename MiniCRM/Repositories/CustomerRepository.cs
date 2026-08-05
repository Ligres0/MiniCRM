using Dapper;
using Microsoft.Data.SqlClient;
using MiniCRM.Models;
using System.Data;

namespace MiniCRM.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly string _connectionString;
        private IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
        public CustomerRepository(IConfiguration configuration)
        {
            _connectionString =
                configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "DefaultConnection bağlantısı bulunamadı.");
        }
        public List<Customers> GetAllActive()
        {
            using var connection = CreateConnection();

            const string sql = """
        SELECT
            Id,
            Name,
            Surname,
            Email,
            Phone,
            CompanyName,
            CreatedDate,
            IsActive
        FROM Customers
        WHERE IsActive = 1
        ORDER BY Name, Surname;
        """;

            return connection.Query<Customers>(sql).ToList();
        }
        public Customers? GetById(int id)
        {
            using var connection = CreateConnection();
            const string sql = """
                SELECT * FROM Customers WHERE Id = @Id
            """;
            return connection.QueryFirstOrDefault<Customers>(
                sql,
                new { Id = id });
        }

        public int Insert(Customers customer)
        {
            using var connection = CreateConnection();

            const string sql = """
        INSERT INTO Customers
        (
            Name,
            Surname,
            Email,
            Phone,
            CompanyName,
            IsActive
        )
        VALUES
        (
            @Name,
            @Surname,
            @Email,
            @Phone,
            @CompanyName,
            @IsActive
        );
        """;

            return connection.Execute(sql, customer);
        }

        public int Update(Customers customer)
        {
            using var connection = CreateConnection();
            const string sql = """
                UPDATE Customers 
                SET Name = @Name, Surname = @Surname, Email = @Email, Phone = @Phone, CompanyName = @CompanyName, CreatedDate = @CreatedDate, IsActive = @IsActive
                WHERE Id = @Id
            """;

            return connection.Execute(sql, customer);
        }
        public int Deactivate(int id)
        {
            using var connection = CreateConnection();
            const string sql = """
                UPDATE Customers 
                SET IsActive = 0
                WHERE Id = @Id
            """;
            return connection.Execute(sql, new { Id = id });
        }

        public List<Customers> GetFilteredPaged(
            string? search,
            string? companyName,
            bool? isActive,
            int pageNumber,
            int pageSize
        )
        {
            using var connection = CreateConnection();
            int offset = (pageNumber - 1) * pageSize;


            const string sql = """
                SELECT * FROM Customers
                WHERE 
                    (@Search IS NULL
            OR Name LIKE '%' + @Search + '%'
            OR Surname LIKE '%' + @Search + '%'
            OR Name + ' ' + Surname LIKE '%' + @Search + '%')
                    AND (@CompanyName IS NULL OR CompanyName LIKE '%' + @CompanyName + '%')
                    AND (@IsActive IS NULL OR IsActive = @IsActive)
                ORDER BY Id
                
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY
            """;

            return connection.Query<Customers>(
                sql,
                new
                {
                    Search = search,
                    CompanyName = companyName,
                    IsActive = isActive,
                    Offset = offset,
                    PageSize = pageSize
                }).ToList();
        }

        public int GetFilteredCount(
            string? search,
            string? companyName,
            bool? isActive
        )
        {
            using var connection = CreateConnection();
            const string sql = """
                SELECT COUNT(*) FROM Customers
                WHERE 
                    (@Search IS NULL
            OR Name LIKE '%' + @Search + '%'
            OR Surname LIKE '%' + @Search + '%'
            OR Name + ' ' + Surname LIKE '%' + @Search + '%')
                    AND (@CompanyName IS NULL OR CompanyName = @CompanyName)
                    AND (@IsActive IS NULL OR IsActive = @IsActive)
            """;
            return connection.ExecuteScalar<int>(
                sql,
                new
                {
                    Search = search,
                    CompanyName = companyName,
                    IsActive = isActive
                });
        }
        public bool EmailExists(string email, int? excludeId = null)
        {
            using var connection = CreateConnection();
            const string sql = """
                SELECT COUNT(*) FROM Customers
                WHERE Email = @Email AND (@ExcludeId IS NULL OR Id != @ExcludeId)
            """;
            int count = connection.ExecuteScalar<int>(
                sql,
                new { Email = email, ExcludeId = excludeId });
            return count > 0;
        }

    }
}
