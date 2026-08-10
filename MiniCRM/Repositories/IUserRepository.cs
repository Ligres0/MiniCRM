using MiniCRM.Models;

namespace MiniCRM.Repositories
{
    public interface IUserRepository
    {
        User? GetByUsername(string username);
    }
}