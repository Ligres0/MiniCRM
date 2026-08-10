namespace MiniCRM.Services
{
    public interface IAuthorizationService
    {
        bool HasPermission(int userId, string permission);
    }
}
