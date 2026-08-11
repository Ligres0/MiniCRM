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
        private readonly IAuthorizationService _authorizationService;

        public DashboardController(IDashboardService dashboardService,IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
            _dashboardService = dashboardService;
        }
        public IActionResult Index()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!_authorizationService.HasPermission(
                userId.Value,
                "Dashboard.View"))
            {
                return Forbid();
            }
            var activeProducts = _dashboardService.GetActiveProducts();
            var totalCustomers = _dashboardService.GetTotalCustomers();
            var todayOrderCount = _dashboardService.GetTodayOrderCount();
            var todayRevenue = _dashboardService.GetTodayRevenue();
            var criticalStockProducts = _dashboardService.GetCriticalStockProducts(100);
            var lastOrders = _dashboardService.GetLastOrders();
            var topSellingProducts = _dashboardService.GetTopSellingProducts();
            var last7DaysRevenue = _dashboardService.GetLast7DaysRevenue();
            var viewModel = new DashboardViewModel
            {
                TotalCustomers = totalCustomers,
                ActiveProducts = activeProducts,
                TodayOrderCount = todayOrderCount,
                TodayRevenue = todayRevenue,
                CriticalStockProducts = criticalStockProducts,
                LastOrders = lastOrders,
                TopSellingProducts = topSellingProducts,
                Last7DaysRevenue = last7DaysRevenue,
            };
            return View(viewModel);
        }
    }
}
