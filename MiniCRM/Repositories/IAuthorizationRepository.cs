namespace MiniCRM.Repositories
{
    public interface IAuthorizationRepository
    {
        bool HasPermission(int userId, string permissionCode);
        List<string> GetUserPermissions(int userId);
    }
}