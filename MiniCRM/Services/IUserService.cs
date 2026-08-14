using MiniCRM.Models;
using MiniCRM.ViewModels;

namespace MiniCRM.Services
{
    public interface IUserService
    {
        bool CreateUser(
            UserCreateViewModel model,
            out string message);

        List<UserListViewModel> GetAllWithRoles();
        UserRoleEditViewModel? GetUserRoleEditModel(int userId);
        bool UpdateUserRoles(
    int userId,
    List<int> roleIds,
    out string message);
        bool HasAnyRole(int userId);
        bool HandleFailedLogin(User user,out string message);

        void HandleSuccessfulLogin(int userId);
    }

}