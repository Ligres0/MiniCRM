using MiniCRM.Models;

namespace MiniCRM.Repositories
{
    public interface IStockMovementRepository
    {
        int Insert(StockMovement stockMovement);
    }
}
