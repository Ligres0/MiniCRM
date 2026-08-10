using Microsoft.AspNetCore.Mvc;
using MiniCRM.Repositories;

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
            if (user.PasswordHash != password) 
            {
                ViewBag.Error = "Şifre yanlış.";
                return View();
            }

            HttpContext.Session.SetInt32("UserId", user.Id);

            return RedirectToAction("Index","Dashboard");
        }
    }
}