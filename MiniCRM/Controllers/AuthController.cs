using Microsoft.AspNetCore.Mvc;
using MiniCRM.Repositories;
using Microsoft.AspNetCore.Identity;
using MiniCRM.Models;

namespace MiniCRM.Controllers
{
    public class AuthController : Controller
    {
        private readonly IUserRepository _userRepository;
        
        public AuthController (IUserRepository userRepository)
        {
            _userRepository = userRepository;
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