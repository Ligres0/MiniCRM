using MiniCRM.Models;
using MiniCRM.ViewModels;
using MiniCRM.ViewModels;

namespace MiniCRM.Repositories
{
    public interface IUserRepository
    {
        User? GetByUsername(string username);
        bool UsernameExists(string username);

        int Insert(User user);
        List<UserListViewModel> GetAllWithRoles();
        List<Role> GetAllRoles();

        List<int> GetUserRoleIds(int userId);
        void UpdateUserRoles(
    int userId,
    List<int> roleIds);
    }
}