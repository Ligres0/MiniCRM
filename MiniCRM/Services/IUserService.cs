using MiniCRM.ViewModels;

namespace MiniCRM.Services
{
    public interface IUserService
    {
        bool CreateUser(
            UserCreateViewModel model,
            out string message);
    }
}