namespace MiniCRM.ViewModels
{
    public class UserListViewModel
    {
        public int Id { get; set; }

        public string UserName { get; set; }
            = string.Empty;

        public string Email { get; set; }
            = string.Empty;

        public bool IsActive { get; set; }

        public DateTime CreatedDate { get; set; }

        public string? RoleNames { get; set; }
    }
}