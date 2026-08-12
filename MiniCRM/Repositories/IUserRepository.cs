using MiniCRM.Models;

namespace MiniCRM.Repositories
{
    public interface IUserRepository
    {
        User? GetByUsername(string username);
        bool UsernameExists(string username);

        int Insert(User user);
    }
}