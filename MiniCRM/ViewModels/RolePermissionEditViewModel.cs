using MiniCRM.Models;

namespace MiniCRM.ViewModels
{
    public class RolePermissionEditViewModel
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public List<Permission> AllPermissions { get; set; } = new List<Permission>();
        public List<int> SelectedPermissionIds { get; set; }= new List<int>();

    }
}
