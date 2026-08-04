using Dapper;
using Microsoft.Data.SqlClient;
using MiniCRM.Models;
using System.Data;

namespace MiniCRM.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly string _connectionString;

        private IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
        public OrderRepository(IConfiguration configuration)
        {
            _connectionString =
                configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "DefaultConnection bağlantısı bulunamadı.");
        }

        public List<Order> GetFilteredPaged(
            string? search,
            int? customerId,
            Order.OrderStatus? status,
            int pageNumber,
            int pageSize)
        {
            using var connection = CreateConnection();
            int offset = (pageNumber - 1) * pageSize;

            const string sql = """
                SELECT *
                FROM Orders
                WHERE (@search IS NULL OR CAST(Id AS NVARCHAR) LIKE '%' + @search + '%')
                  AND (@customerId IS NULL OR CustomerId = @customerId)
                  AND (@status IS NULL OR Status = @status)
                ORDER BY Id
                OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;
                """;
            return connection.Query<Order>(sql, new { search, customerId, status, offset, pageSize }).ToList();


        }


        public int GetFilteredCount(
            string? search,
            int? customerId,
            Order.OrderStatus? status)
        {
            using var connection = CreateConnection();
            const string sql = """
                SELECT COUNT(*)
                FROM Orders
                WHERE (@search IS NULL OR CAST(Id AS NVARCHAR) LIKE '%' + @search + '%')
                  AND (@customerId IS NULL OR CustomerId = @customerId)
                  AND (@status IS NULL OR Status = @status);
                """;
            return connection.ExecuteScalar<int>(sql, new { search, customerId, status });
        }


        public Order? GetById(int id)
        {
            using var connection = CreateConnection();
            const string sql = "SELECT * FROM Orders WHERE Id = @id";
            return connection.QuerySingleOrDefault<Order>(sql, new { id });
        }
        public List<OrderDetails> GetDetailsByOrderId(int orderId)
        {
            using var connection = CreateConnection();
            const string sql = "SELECT * FROM OrderDetails WHERE OrderId = @orderId";
            return connection.Query<OrderDetails>(sql, new { orderId }).ToList();
        }

        public int InsertOrder(Order order)
        {
            using var connection = CreateConnection();
            const string sql = """
                INSERT INTO Orders (CustomerId, CreatedByUserId, OrderDate, TotalAmount, Status)
                VALUES (@CustomerId, @CreatedByUserId, @OrderDate, @TotalAmount, @Status);
                SELECT CAST(SCOPE_IDENTITY() AS INT);
                """;
            return connection.ExecuteScalar<int>(sql, order);
        }
        public int InsertOrderDetail(OrderDetails detail)
        {
            using var connection = CreateConnection();
            const string sql = """
                INSERT INTO OrderDetails (OrderId, ProductId, Quantity, UnitPrice, TotalPrice)
                VALUES (@OrderId, @ProductId, @Quantity, @UnitPrice, @TotalPrice);
                SELECT CAST(SCOPE_IDENTITY() AS INT);
                """;
            return connection.ExecuteScalar<int>(sql, detail);
        }
        public int Update(Order order)
        {
            using var connection = CreateConnection();
            const string sql = """
                UPDATE Orders
                SET CustomerId = @CustomerId,
                    CreatedByUserId = @CreatedByUserId,
                    OrderDate = @OrderDate,
                    TotalAmount = @TotalAmount,
                    Status = @Status
                WHERE Id = @Id;
                """;
            return connection.Execute(sql, order);
        }
        public int UpdateStatus(int orderId, Order.OrderStatus status)
        {
            using var connection = CreateConnection();
            const string sql = """
                UPDATE Orders
                SET Status = @Status
                WHERE Id = @Id;
                """;
            return connection.Execute(sql, new { Id = orderId, Status = status });
        }

        }

    }


