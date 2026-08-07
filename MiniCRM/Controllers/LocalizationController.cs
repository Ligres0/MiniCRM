using Microsoft.AspNetCore.Mvc;

namespace MiniCRM.Controllers
{
    public class LocalizationController: Controller
    {
        public IActionResult ChangeLanguage(
            string culture,
            string returnUrl)
        {
            if (culture != "tr-TR" && culture != "en-US")
            {
                culture = "en-US";
            }

            Response.Cookies.Append(
                "Culture",
                culture,
                new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddYears(1)
                });

            return LocalRedirect(returnUrl);
        }




    }
}
