using AspNetCoreGeneratedDocument;
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
        bool EmailExists(string email);
        bool HasAnyRole(int userId);

        void IncrementFailedLoginAttempts(int userId);
        void LockUser(int userId,DateTime lockoutEnd);
        void ResetLoginAttempts(int userId);
    }
}