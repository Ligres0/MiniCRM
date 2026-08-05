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
        public int DeleteDetailsByOrderId(int orderId)
        {
            using var connection = CreateConnection();

            const string sql = """
        DELETE FROM OrderDetails
        WHERE OrderId = @OrderId;
        """;

            return connection.Execute(
                sql,
                new { OrderId = orderId });
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
        public int CreateOrderTransaction(Order order, List<OrderDetails> details, int createdByUserId)
        {
            using var connection = CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                order.CreatedByUserId = createdByUserId;
                const string orderSql = """
                INSERT INTO Orders (CustomerId, CreatedByUserId, OrderDate, TotalAmount, Status)
                VALUES (@CustomerId, @CreatedByUserId, @OrderDate, @TotalAmount, @Status);
                SELECT CAST(SCOPE_IDENTITY() AS INT);
                """;

                int orderId = connection.ExecuteScalar<int>(orderSql, order, transaction);

                if (orderId <= 0)
                {
                    throw new Exception("Order could not be created.");
                }

                foreach (var detail in details)
                {
                    detail.OrderId = orderId;

                    const string detailSql = """
                    INSERT INTO OrderDetails (OrderId, ProductId, Quantity, UnitPrice, TotalPrice)
                    VALUES (@OrderId, @ProductId, @Quantity, @UnitPrice, @TotalPrice); 
                    
                    """;

                    int affectedRows = connection.Execute(detailSql, detail, transaction);
                    if (affectedRows <= 0)
                    {
                        throw new Exception("Order detail could not be created.");
                    }

                    const string updateProductSql = """
                    UPDATE Products 
                        SET Stock = Stock - @Quantity
                    WHERE Id = @ProductId AND Stock >= @Quantity AND IsActive = 1;
                    """;

                    int affectedtransactionRows = connection.Execute(updateProductSql, detail, transaction);
                    if (affectedtransactionRows <= 0)
                    {
                        throw new Exception("Product stock could not be updated.");
                    }

                    var stockMovement = new StockMovement
                    {
                        ProductId = detail.ProductId,
                        Quantity = detail.Quantity,
                        MovementType = StockMovement.MovementTypeenum.Out,
                        CreatedDate = DateTime.Now,
                        CreatedByUserId = createdByUserId,
                        OrderId = orderId
                    };

                    const string stockMovementSql = """
                    Insert INTO StockMovements (ProductId, OrderId, MovementType, Quantity, CreatedDate, CreatedByUserId)
                    VALUES (@ProductId, @OrderId, @MovementType, @Quantity, @CreatedDate, @CreatedByUserId);
                    """;

                    int affectedRowss = connection.Execute(stockMovementSql, stockMovement, transaction);
                    if (affectedRowss <= 0)
                    {
                        throw new Exception("Stock movement could not be created.");
                    }

                }
                transaction.Commit();
                return orderId;






            }

            catch
            {
                transaction.Rollback();
                throw;
            }




        }

        public int CancelOrderTransaction(int orderId, int cancelledByUserId)
        {
            using var connection = CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();
            try
            {
                const string detailSql = """

                SELECT * FROM OrderDetails WHERE OrderId = @OrderId;
                """;
                var orderDetails = connection.Query<OrderDetails>(detailSql, new { OrderId = orderId }, transaction).ToList();

                if(orderDetails.Count ==0)
                {
                    throw new Exception("No order details found for the given order.");
                }
                foreach (var detail in orderDetails)
                {
                    detail.OrderId = orderId;

                    const string updateProductSql = """
                    UPDATE Products 
                        SET Stock = Stock + @Quantity
                    WHERE Id = @ProductId AND IsActive = 1;
                    """;

                    var affectedtransactionRows = connection.Execute(updateProductSql, detail, transaction);

                    if(affectedtransactionRows <= 0)
                    {
                        throw new Exception("Product stock could not be updated.");
                    }
                    var stockMovement = new StockMovement
                    {
                        ProductId = detail.ProductId,
                        Quantity = detail.Quantity,
                        MovementType = StockMovement.MovementTypeenum.In,
                        CreatedDate = DateTime.Now,
                        CreatedByUserId = cancelledByUserId,
                        OrderId = orderId
                    };

                    const string stockMovementSql = """
                    Insert INTO StockMovements (ProductId, OrderId, MovementType, Quantity, CreatedDate, CreatedByUserId)
                    VALUES (@ProductId, @OrderId, @MovementType, @Quantity, @CreatedDate, @CreatedByUserId);
                    """;

                    int affectedRowss = connection.Execute(stockMovementSql, stockMovement, transaction);
                    if (affectedRowss <= 0)
                    {
                        throw new Exception("Stock movement could not be created.");
                    }

                }
                const string updateOrderStatusSql = """
                UPDATE Orders
                SET Status = @CancelledStatus
                WHERE Id = @OrderId AND Status != @CancelledStatus;
                """;
                int affectedOrderRows = connection.Execute(
                    updateOrderStatusSql,
                    new
                    {
                        OrderId = orderId,
                        CancelledStatus = Order.OrderStatus.Cancelled,
                        DraftStatus = Order.OrderStatus.Draft
                    },
                    transaction);

                if (affectedOrderRows <= 0)
                {
                    throw new Exception(
                        "Order could not be cancelled.");
                }






                transaction.Commit();
                return orderId;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }

    }


