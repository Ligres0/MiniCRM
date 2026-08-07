using MiniCRM.Repositories;
using Microsoft.Extensions.Caching.Memory;

namespace MiniCRM.Services
{
    public class LocalizationService: ILocalizationService
    {
        private readonly IMemoryCache _cache;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILocalizationRepository _localizationRepository;
        public LocalizationService(ILocalizationRepository localizationRepository, IHttpContextAccessor httpContextAccesor, IMemoryCache cache)
        {
            _localizationRepository = localizationRepository;
            _httpContextAccessor = httpContextAccesor;
            _cache = cache;

        }
        public string GetText(string key)
        {
            var culture = _httpContextAccessor
                .HttpContext?
                .Request
                .Cookies["Culture"];

            if (string.IsNullOrEmpty(culture))
            {
                culture = "en-US";
            }

            string cacheKey = $"Localization:{culture}";

            if (!_cache.TryGetValue(
                cacheKey,
                out Dictionary<string, string>? dictionary))
            {
                var localizations =
                    _localizationRepository.GetByCulture(culture);

                dictionary = localizations.ToDictionary(
                    x => x.Key,
                    x => x.Value);

                _cache.Set(
                    cacheKey,
                    dictionary,
                    TimeSpan.FromMinutes(30));
            }

            if (dictionary != null &&
                dictionary.TryGetValue(key, out var value))
            {
                return value;
            }

            return key;

            
        }
        public void ClearCache(string culture) 
        {
            string cacheKey = $"Localization:{culture}";
            _cache.Remove(cacheKey);
        }
    }
}
