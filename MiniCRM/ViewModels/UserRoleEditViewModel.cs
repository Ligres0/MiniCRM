using MiniCRM.Models;

namespace MiniCRM.ViewModels
{
    public class UserRoleEditViewModel
    {
        public int UserId { get; set; }

        public string UserName { get; set; }
            = string.Empty;

        public List<Role> AllRoles { get; set; }
            = new List<Role>();

        public List<int> SelectedRoleIds { get; set; }
            = new List<int>();
    }
}