using MiniCRM.Models;

namespace MiniCRM.Services
{
    public interface IDashboardService
    {
        int GetTotalCustomers();
        int GetActiveProducts();
        int GetTodayOrderCount();
        decimal GetTodayRevenue();
        List<Product> GetCriticalStockProducts(int criticalStockLevel);
    }
}
