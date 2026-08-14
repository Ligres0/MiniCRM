namespace MiniCRM.Models
{
    public class UserActivityLog
    {
        public int Id { get; set; }

        public int? UserId { get; set; }

        public string Action { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}