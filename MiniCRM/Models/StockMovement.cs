
namespace MiniCRM.Models
{
    public class StockMovement
    {
        public enum MovementTypeenum
        {
            In = 1,
            Out = 2
        }

        public int Id { get; set; }
        public int ProductId { get; set; }
        public int OrderId { get; set; }
        public MovementTypeenum MovementType { get; set; }

        public int Quantity { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedByUserId { get; set; }
    }
}
