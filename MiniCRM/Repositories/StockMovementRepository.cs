using Microsoft.Data.SqlClient;
using System.Data;
using Dapper;
using MiniCRM.Models;

namespace MiniCRM.Repositories
{
    public class StockMovementRepository : IStockMovementRepository
    {
        private readonly string _connectionString;

        private IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public StockMovementRepository(IConfiguration configuration)
        {
            _connectionString =
                configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "DefaultConnection bağlantısı bulunamadı.");
        }

        public int Insert(StockMovement stockMovement)
        {
            using var connection = CreateConnection();
            const string sql = """
                INSERT INTO StockMovements (ProductId, Quantity, MovementType, CreatedDate, CreatedByUserId, OrderId)
                VALUES (@ProductId, @Quantity, @MovementType, @CreatedDate, @CreatedByUserId, @OrderId);
                SELECT CAST(SCOPE_IDENTITY() AS INT);
                """;
            return connection.Execute(sql, stockMovement);
        }
        
    }
}
