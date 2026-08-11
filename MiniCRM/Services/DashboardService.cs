using MiniCRM.Repositories;
using MiniCRM.Models;
using MiniCRM.ViewModels;
using Microsoft.Extensions.Caching.Memory;

namespace MiniCRM.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IDashboardRepository _dashboardRepository;
        private readonly IMemoryCache _cache;

        public DashboardService(
            IDashboardRepository dashboardRepository, IMemoryCache cache)
        {
            _dashboardRepository = dashboardRepository;
            _cache = cache;
        }

        public int GetTotalCustomers()
        {
            string cacheKey = "Dashboard:TotalCustomers";

            if (!_cache.TryGetValue(cacheKey, out int totalCustomers))
            {
                totalCustomers = _dashboardRepository.GetTotalCustomers();

                _cache.Set(cacheKey, totalCustomers, TimeSpan.FromMinutes(5));
            }

            return totalCustomers;
        }
        public int GetActiveProducts()
        {
            string cacheKey = "Dashboard:ActiveProducts";

            if(!_cache.TryGetValue(cacheKey,out int activeProducts))
            {
                activeProducts = _dashboardRepository.GetActiveProducts();

                _cache.Set(cacheKey,activeProducts, TimeSpan.FromMinutes(5));
            }
            return activeProducts;
        }
        public int GetTodayOrderCount()
        {
            string cacheKey = "Dashboard:TodayOrderCount";

            if(!_cache.TryGetValue(cacheKey, out int todayOrderCount))
            {
                todayOrderCount = _dashboardRepository.GetTodayOrderCount();
                _cache.Set(cacheKey,todayOrderCount,TimeSpan.FromMinutes(2));
            }
            return todayOrderCount;
        }
        public decimal GetTodayRevenue()
        {
            string cacheKey = "Dashboard:TodayRevenue";

            if (!_cache.TryGetValue(cacheKey, out decimal todayRevenue))
            {
                todayRevenue = _dashboardRepository.GetTodayRevenue();
                _cache.Set(cacheKey,todayRevenue,TimeSpan.FromMinutes(2));
            }
            return todayRevenue;
        }
        public List<Product> GetCriticalStockProducts(int criticalStockLevel)
        {
            string cacheKey = $"Dashboard:CriticalStock:{criticalStockLevel}";

            if(!_cache.TryGetValue(cacheKey,out List<Product>? products))
            {
                products = _dashboardRepository.GetCriticalStockProducts(criticalStockLevel);

                _cache.Set(cacheKey,products,TimeSpan.FromMinutes(2));
            }
            return products ?? new List<Product>();
        }
        public List<Order> GetLastOrders()
        {
            string cacheKey = "Dashboard:LastOrders";

            if(!_cache.TryGetValue(cacheKey,out List<Order>? orders))
            {
                orders = _dashboardRepository.GetLastOrders();

                _cache.Set(cacheKey,orders,TimeSpan.FromMinutes(2));
            }
            return orders?? new List<Order>();
        }
        public List<TopSellingProductViewModel> GetTopSellingProducts()
        {
            string cacheKey = "Dashboard:TopSellingProducts";

            if(!_cache.TryGetValue(cacheKey,out List<TopSellingProductViewModel>? products))
            {
                products = _dashboardRepository.GetTopSellingProducts();

                _cache.Set(cacheKey,products ,TimeSpan.FromMinutes(5));
            }
            return products?? new List<TopSellingProductViewModel>();
        }
        public void ClearDashboardCache()
        {
            _cache.Remove("Dashboard:TotalCustomers");
            _cache.Remove("Dashboard:ActiveProducts");
            _cache.Remove("Dashboard:TodayOrderCount");
            _cache.Remove("Dashboard:TodayRevenue");
            _cache.Remove("Dashboard:CriticalStock:100");
            _cache.Remove("Dashboard:LastOrders");
            _cache.Remove("Dashboard:TopSellingProducts");
        }
    }
}