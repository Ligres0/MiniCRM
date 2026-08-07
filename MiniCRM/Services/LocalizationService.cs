using MiniCRM.Repositories;

namespace MiniCRM.Services
{
    public class LocalizationService: ILocalizationService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILocalizationRepository _localizationRepository;
        public LocalizationService(ILocalizationRepository localizationRepository, IHttpContextAccessor httpContextAccesor)
        {
            _localizationRepository = localizationRepository;
            _httpContextAccessor = httpContextAccesor;

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

            var localizations =
                _localizationRepository.GetByCulture(culture);

            var localization =
                localizations.FirstOrDefault(x => x.Key == key);

            if (localization == null)
            {
                return key;
            }

            return localization.Value;
        }
    }
}
