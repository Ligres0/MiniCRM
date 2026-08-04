using MiniCRM.Models;

namespace MiniCRM.ViewModels
{
    public class OrderListViewModel
    {
        public List<Order> Orders { get; set; } = new();

        // Müşteri filtresi dropdown'u için.
        public List<Customers> Customers { get; set; } = new();

        public string? Search { get; set; }

        public int? CustomerId { get; set; }

        public Order.OrderStatus? Status { get; set; }

        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }

        public int TotalCount { get; set; }

        public int PageSize { get; set; }
    }
}