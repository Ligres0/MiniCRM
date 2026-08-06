using MiniCRM.Repositories;
using MiniCRM.Models;

namespace MiniCRM.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IDashboardRepository _dashboardRepository;

        public DashboardService(
            IDashboardRepository dashboardRepository)
        {
            _dashboardRepository = dashboardRepository;
        }

        public int GetTotalCustomers()
        {
            return _dashboardRepository.GetTotalCustomers();
        }
        public int GetActiveProducts()
        {
            return _dashboardRepository.GetActiveProducts();
        }
        public int GetTodayOrderCount()
        {
            return _dashboardRepository.GetTodayOrderCount();
        }
        public decimal GetTodayRevenue()
        {
            return _dashboardRepository.GetTodayRevenue();
        }
        public List<Product> GetCriticalStockProducts(int criticalStockLevel)
        {
            return _dashboardRepository.GetCriticalStockProducts(criticalStockLevel);
        }
    }
}