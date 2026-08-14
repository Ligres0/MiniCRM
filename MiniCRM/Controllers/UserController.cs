using Microsoft.AspNetCore.Mvc;
using MiniCRM.Services;
using MiniCRM.ViewModels;

namespace MiniCRM.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IAuthorizationService _authorizationService;
        private readonly IUserActivityLogService _activityLogService;

        public UserController(IUserService userService, IAuthorizationService authorizationService, IUserActivityLogService activityLogService)
        {
            _userService = userService;
            _authorizationService = authorizationService;
            _activityLogService = activityLogService;
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
                "User.Create"))
            {
                return StatusCode(
                        StatusCodes.Status403Forbidden);
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
                "User.Create"))
            {
                return StatusCode(
                        StatusCodes.Status403Forbidden);
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

            _activityLogService.Log(
                userId.Value,
                "UserCreated",
                $"{model.UserName} kullanıcısı oluşturuldu.");

            TempData["SuccessMessage"] =
                message;

            return RedirectToAction(
                nameof(Index));

        }

        [HttpGet]
        public IActionResult Index()
        {
            int?userId = HttpContext.Session.GetInt32("UserId");
            if(!userId.HasValue)
            {
                return RedirectToAction("Login", "Auth");
            }

            if(!_authorizationService.HasPermission(userId.Value,"User.View"))
            {
                return StatusCode(StatusCodes.Status403Forbidden);
            }
            var users = _userService.GetAllWithRoles();

            return View(users);
        }
        [HttpGet]
        public IActionResult AssignRoles(int id)
        {
            int? userId =
                HttpContext.Session.GetInt32("UserId");

            if (!userId.HasValue)
            {
                return RedirectToAction(
                    "Login",
                    "Auth");
            }

            if (!_authorizationService.HasPermission(
                userId.Value,
                "User.Edit"))
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden);
            }

            var model =
                _userService.GetUserRoleEditModel(id);

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AssignRoles(UserRoleEditViewModel model)
        {
            int? currentUserId =
                HttpContext.Session.GetInt32("UserId");

            if (!currentUserId.HasValue)
            {
                return RedirectToAction(
                    "Login",
                    "Auth");
            }

            if (!_authorizationService.HasPermission(
                currentUserId.Value,
                "User.Edit"))
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden);
            }

            model.SelectedRoleIds ??=
                new List<int>();

            bool result =
                _userService.UpdateUserRoles(
                    model.UserId,
                    model.SelectedRoleIds,
                    out string message);

            if (!result)
            {
                ModelState.AddModelError(
                    string.Empty,
                    message);

                var refreshedModel =
                    _userService.GetUserRoleEditModel(
                        model.UserId);

                if (refreshedModel == null)
                {
                    return NotFound();
                }

                refreshedModel.SelectedRoleIds =
                    model.SelectedRoleIds;

                return View(refreshedModel);
            }
            _activityLogService.Log(
                    currentUserId.Value,
                    "UserRolesUpdated",
                     $"{model.UserName} kullanıcısının rolleri değiştirildi.");

            TempData["SuccessMessage"] =
                message;

            return RedirectToAction(
                nameof(Index));
        }
    }
}
