using Dapper;
using Microsoft.Data.SqlClient;
using MiniCRM.Models;
using System.Data;

namespace MiniCRM.Repositories
{
    public class DashboardRepository: IDashboardRepository
    {
        private readonly string _connectionString;

        public DashboardRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("DefaultConnection bağlantısı bulunamadı.");
        }

        private IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public int GetTotalCustomers()
        {
            using var connection = CreateConnection();
            const string sql = "SELECT COUNT(*) FROM Customers";
            return connection.ExecuteScalar<int>(sql);
        }

        public int GetActiveProducts()
        {
            using var connection = CreateConnection();
            const string sql = "SELECT COUNT(*) FROM Products WHERE IsActive = 1";
            return connection.ExecuteScalar<int>(sql);
        }

        public int GetTodayOrderCount()
        {
            using var connection = CreateConnection();
            const string sql = """
                SELECT COUNT(*)
                FROM Orders
                WHERE CAST(OrderDate AS DATE) = CAST(GETDATE() AS DATE);
                """;
            return connection.ExecuteScalar<int>(sql);
        }
        public decimal GetTodayRevenue()
        {
            using var connection = CreateConnection();
            const string sql = """
                SELECT SUM(TotalAmount)
                FROM Orders
                WHERE CAST(OrderDate AS DATE) = CAST(GETDATE() AS DATE)
                AND Status = @CompletedStatus;
                """;
            return connection.ExecuteScalar<decimal>(
                sql,
                new
                {
                    CompletedStatus = Order.OrderStatus.Completed
                });
        }

        public List<Product> GetCriticalStockProducts(int criticalStockLevel)
        {
            using var connection = CreateConnection();
            const string sql = "SELECT * FROM Products WHERE Stock <= @CriticalStockLevel AND IsActive = 1 ORDER BY Stock ASC";
            return connection.Query<Product>(sql, new { CriticalStockLevel = criticalStockLevel }).ToList();
        }
    }
    
}
