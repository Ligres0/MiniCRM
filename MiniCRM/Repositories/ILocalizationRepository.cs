using MiniCRM.Models;

namespace MiniCRM.Repositories
{
    public interface ILocalizationRepository
    {
        List<Localization> GetByCulture(string culture);
    }
}
