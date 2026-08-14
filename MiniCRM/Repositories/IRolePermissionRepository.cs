using MiniCRM.Models;

namespace MiniCRM.Repositories
{
    public interface IRolePermissionRepository
    {
        List<Permission> GetAllPermissions();
        List<int> GetRolePermissionIds(int roleId);

        void UpdateRolePermission(int roleId, List<int> permissionIds);
        List<int> GetUserIdsByRoleId(int roleId);
        bool RoleNameExists(string name);

        int InsertRole(string name);
    }
}
