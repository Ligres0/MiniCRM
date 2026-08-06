using Microsoft.AspNetCore.Mvc;
using MiniCRM.Models;
using MiniCRM.Repositories;
using MiniCRM.Services;
using MiniCRM.ViewModels;

namespace MiniCRM.Controllers
{
    public class DashboardController: Controller
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }
        public IActionResult Index()
        {
            var activeProducts = _dashboardService.GetActiveProducts();
            var totalCustomers = _dashboardService.GetTotalCustomers();
            var todayOrderCount = _dashboardService.GetTodayOrderCount();
            var todayRevenue = _dashboardService.GetTodayRevenue();
            var criticalStockProducts = _dashboardService.GetCriticalStockProducts(100); 
            var viewModel = new DashboardViewModel
            {
                TotalCustomers = totalCustomers,
                ActiveProducts = activeProducts,
                TodayOrderCount = todayOrderCount,
                TodayRevenue = todayRevenue
            };
            return View(viewModel);
        }
    }
}
