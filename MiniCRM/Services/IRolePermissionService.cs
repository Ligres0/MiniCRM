using MiniCRM.Models;
using MiniCRM.ViewModels;


namespace MiniCRM.Services
{
    public interface IRolePermissionService
    {
        RolePermissionEditViewModel? GetRolePermissionEditModel(int roleId);


        bool UpdateRolePermissions(
            int roleId,
            List<int> permissionIds,
            out string message);

        List<Role> GetAllRoles();

        bool CreateRole(RoleCreateViewModel model, out int roleId, out string message);
    }
}
