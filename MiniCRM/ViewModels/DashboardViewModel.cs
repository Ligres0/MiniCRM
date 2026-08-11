
using MiniCRM.Models;

namespace MiniCRM.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalCustomers { get; set; }
        public int ActiveProducts { get; set; }
        public int TodayOrderCount { get; set; }
        public decimal TodayRevenue { get; set; }
        public List<Product> CriticalStockProducts { get; set; } = new List<Product>();
        public List<Order> LastOrders { get; set; } 
        public List<TopSellingProductViewModel> TopSellingProducts { get; set; } = new List<TopSellingProductViewModel>();
        public List<DailyRevenueViewModel> Last7DaysRevenue { get; set; } = new List<DailyRevenueViewModel>();

    }
}