using MiniCRM.Repositories;
using Microsoft.Extensions.Caching.Memory;

namespace MiniCRM.Services
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly IAuthorizationRepository _authorizationRepository;
        private readonly IMemoryCache _cache;


        public AuthorizationService(IAuthorizationRepository authorizationRepository, IMemoryCache cache)
        {
            _authorizationRepository = authorizationRepository;
            _cache = cache;
        }
        public bool HasPermission(int userId, string permissionCode)
        {
            string cacheKey = $"UserPermissions:{userId}";

            if (!_cache.TryGetValue(
                cacheKey,
                out List<string>? permissions))
            {
                permissions =
                    _authorizationRepository.GetUserPermissions(userId);

                _cache.Set(
                    cacheKey,
                    permissions,
                    TimeSpan.FromMinutes(10));
            }

            return permissions!.Contains(permissionCode);
        }

        public void ClearPermissionCache(int userId) 
        {
            string cacheKey =  $"UserPermissions:{userId}";

            _cache.Remove(cacheKey);

        }





    }
}
