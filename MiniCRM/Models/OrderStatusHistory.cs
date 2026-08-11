namespace MiniCRM.Models
{
    public class OrderStatusHistory
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public Order.OrderStatus? OldStatus { get; set; }

        public Order.OrderStatus NewStatus { get; set; }

        public int ChangedByUserId { get; set; }

        public DateTime ChangedAt { get; set; }
    }
}