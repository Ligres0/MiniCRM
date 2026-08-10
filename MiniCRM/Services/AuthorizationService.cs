using MiniCRM.Repositories;

namespace MiniCRM.Services
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly IAuthorizationRepository _authorizationRepository;


        public AuthorizationService(IAuthorizationRepository authorizationRepository)
        {
            _authorizationRepository = authorizationRepository;
        }

        public bool HasPermission(int userId, string permissionCode)
        {
            return _authorizationRepository.HasPermission(userId, permissionCode);

        }
    }
}
