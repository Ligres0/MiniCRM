using MiniCRM.Models;
using MiniCRM.Repositories;
using MiniCRM.ViewModels;


namespace MiniCRM.Services
{
    public class RolePermissionService: IRolePermissionService
    {
        private readonly IRolePermissionRepository _rolePermissionRepository;
        private readonly IUserRepository _userRepository;
        private readonly IAuthorizationService _authorizationService;

        public RolePermissionService(
           IRolePermissionRepository rolePermissionRepository,
           IUserRepository userRepository,
           IAuthorizationService authorizationService)
        {
            _rolePermissionRepository =
                rolePermissionRepository;

            _userRepository =
                userRepository;

            _authorizationService =
                authorizationService;
        }

        public RolePermissionEditViewModel? GetRolePermissionEditModel(int roleId)
        {
            var roles =
                _userRepository.GetAllRoles();

            var role =
                roles.FirstOrDefault(
                    x => x.Id == roleId);

            if (role == null)
            {
                return null;
            }

            var allPermissions =
                _rolePermissionRepository
                    .GetAllPermissions();

            var selectedPermissionIds =
                _rolePermissionRepository
                    .GetRolePermissionIds(roleId);

            return new RolePermissionEditViewModel
            {
                RoleId = role.Id,
                RoleName = role.Name,
                AllPermissions = allPermissions,
                SelectedPermissionIds =
                    selectedPermissionIds
            };
        }

        public bool UpdateRolePermissions(int roleId, List<int> permissionIds, out string message)
        {
            if (roleId <= 0) 
            {
                message = "Invalid role.";
                return false;
            }
            permissionIds ??=
                new List<int>();


            try
            {
                _rolePermissionRepository
                    .UpdateRolePermission(
                        roleId,
                        permissionIds);


                var userIds =
                    _rolePermissionRepository
                        .GetUserIdsByRoleId(
                            roleId);


                foreach (int userId in userIds)
                {
                    _authorizationService
                        .ClearPermissionCache(
                            userId);
                }


                message =
                    "Role permissions updated successfully.";

                return true;
            }
            catch
            {
                message =
                    "Role permissions could not be updated.";

                return false;
            }
        }
        public List<Role> GetAllRoles()
        {
            return _userRepository.GetAllRoles();
        }

        public bool CreateRole(RoleCreateViewModel model, out int roleId, out string message)
        {
            roleId = 0;

            string roleName = model.Name.Trim();

            if (_rolePermissionRepository.RoleNameExists(roleName))
            {
                message = "Role already exists.";
                return false;

            }
            roleId = _rolePermissionRepository.InsertRole(roleName);

            if(roleId <= 0)
            {
                message = "Role could not be created. ";
                return false;
            }

            message = "Role created successfully. ";

            return true;
        }
    }
}

