using Microsoft.AspNetCore.Mvc;
using MiniCRM.Repositories;
using Microsoft.AspNetCore.Identity;
using MiniCRM.Models;
using MiniCRM.Services;

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
        public IActionResult Login(string username, string password) 
        {
            var user = _userRepository.GetByUsername(username);

            if (user == null)
            {
                ViewBag.Error = "Kullanıcı bulunamadı.";
                return View();
            }
            var passwordHasher = new PasswordHasher<User>();

            var result = passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                password
            );

            if (result == PasswordVerificationResult.Failed)
            {
                ViewBag.Error = "Şifre yanlış.";
                return View();
            }
            if (!_userService.HasAnyRole(user.Id))
            {
                ModelState.AddModelError(
                    "",
                    "Hesabınıza henüz bir rol atanmadı. Lütfen sistem yöneticisi ile iletişime geçin.");

                return View();
            }
            HttpContext.Session.SetInt32("UserId", user.Id);

            return RedirectToAction("Index","Dashboard");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Login", "Auth");
        }
    }
}