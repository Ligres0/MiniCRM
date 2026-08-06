
using MiniCRM.ViewModels;
using MiniCRM.Models;

namespace MiniCRM.Repositories
{
    public interface IDashboardRepository
    {
        int GetTotalCustomers();
        int GetActiveProducts();
        int GetTodayOrderCount();
        decimal GetTodayRevenue();
        List<Product> GetCriticalStockProducts(int criticalStockLevel);
        List<Order> GetLastOrders();
        List<TopSellingProductViewModel> GetTopSellingProducts();

    }
}
