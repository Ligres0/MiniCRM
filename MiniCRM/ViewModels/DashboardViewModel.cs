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
    }
}