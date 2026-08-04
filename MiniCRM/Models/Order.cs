using MiniCRM.Models;

namespace MiniCRM.Models
{
    public class Order
    {
        public enum OrderStatus
        {
            Draft = 0,
            Completed = 1,
            Cancelled = 2

        }

        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int CreatedByUserId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
        

    }
}
