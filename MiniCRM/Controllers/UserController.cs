using Microsoft.AspNetCore.Mvc;
using MiniCRM.Services;
using MiniCRM.ViewModels;

namespace MiniCRM.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IAuthorizationService _authorizationService;

        public UserController(IUserService userService, IAuthorizationService authorizationService)
        {
            _userService = userService;
            _authorizationService = authorizationService;
        }

        [HttpGet]
        public IActionResult Create()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Auth");
            }
            if (!_authorizationService.HasPermission(
                userId.Value,
                "User.Manage"))
            {
                return Forbid();
            }

            return View(
                new UserCreateViewModel());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult Create(UserCreateViewModel model)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (!userId.HasValue) 
            {
                return RedirectToAction("Login", "Auth");
                
            }

            if (!_authorizationService.HasPermission(
                userId.Value,
                "User.Manage"))
            {
                return Forbid();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            bool result =
                _userService.CreateUser(
                    model,
                    out string message);

            if (!result)
            {
                ModelState.AddModelError(
                    string.Empty,
                    message);

                return View(model);
            }

            TempData["SuccessMessage"] =
                message;

            return RedirectToAction(
                nameof(Create));

        }
        }
}
