using MiniCRM.Models;
using MiniCRM.ViewModels;

namespace MiniCRM.Services
{
    public interface IDashboardService
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
