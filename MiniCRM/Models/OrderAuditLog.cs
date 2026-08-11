namespace MiniCRM.Models
{
    public class OrderAuditLog
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public string ActionType { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public int ChangedByUserId { get; set; }

        public DateTime ChangedAt { get; set; }
    }
}