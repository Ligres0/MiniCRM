using MiniCRM.Models;

namespace MiniCRM.ViewModels
{
    public class OrderDetailsViewModel
    {
        public Order Order { get; set; } = new();
        public List<OrderDetails> Details { get; set; }
        public Customers Customer { get; set; }

        public string CustomerName { get; set; } = string.Empty;

        public string? CreatedByUserName { get; set; }

        public List<OrderItemViewModel> Items { get; set; } = new();
        public List<OrderAuditLog> AuditLogs { get; set; }
    = new List<OrderAuditLog>();
    }
}