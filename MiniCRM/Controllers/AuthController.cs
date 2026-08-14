using Microsoft.AspNetCore.Mvc;
using MiniCRM.Repositories;
using Microsoft.AspNetCore.Identity;
using MiniCRM.Models;
using MiniCRM.Services;
using Microsoft.AspNetCore.RateLimiting;

namespace MiniCRM.Controllers
{
    public class AuthController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserService _userService;
        
        public AuthController (IUserRepository userRepository, IUserService userService)
        {
            _userRepository = userRepository;
            _userService = userService;
        } 

        
        
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

     
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Login", "Auth");
        }

        [HttpPost]
        [EnableRateLimiting("LoginPolicy")]
        public IActionResult Login(
     string username,
     string password)
        {
            var user =
                _userRepository.GetByUsername(username);

            if (user == null)
            {
                ViewBag.Error =
                    "Kullanıcı bulunamadı.";

                return View();
            }


            // Hesap şu anda kilitli mi?
            if (user.LockoutEnd.HasValue &&
                user.LockoutEnd.Value > DateTime.Now)
            {
                TimeSpan remainingTime =
                    user.LockoutEnd.Value -
                    DateTime.Now;

                int remainingMinutes =
                    (int)Math.Ceiling(
                        remainingTime.TotalMinutes);

                ViewBag.Error =
                    $"Hesap geçici olarak kilitli. " +
                    $"Yaklaşık {remainingMinutes} dakika sonra tekrar deneyin.";

                return View();
            }


            // Şifreyi hash üzerinden kontrol et
            var passwordHasher =
                new PasswordHasher<User>();

            var result =
                passwordHasher.VerifyHashedPassword(
                    user,
                    user.PasswordHash,
                    password);


            // Şifre yanlışsa başarısız denemeyi artır
            if (result ==
                PasswordVerificationResult.Failed)
            {
                _userService.HandleFailedLogin(
                    user,
                    out string message);

                ViewBag.Error =
                    message;

                return View();
            }


            // Rol atanmamış kullanıcı sisteme giremez
            if (!_userService.HasAnyRole(
                user.Id))
            {
                ModelState.AddModelError(
                    "",
                    "Hesabınıza henüz bir rol atanmadı. " +
                    "Lütfen sistem yöneticisi ile iletişime geçin.");

                return View();
            }


            // Başarılı girişte eski başarısız denemeleri sıfırla
            _userService.HandleSuccessfulLogin(
                user.Id);


            // Session oluştur
            HttpContext.Session.SetInt32(
                "UserId",
                user.Id);


            return RedirectToAction(
                "Index",
                "Dashboard");
        }
    }
}