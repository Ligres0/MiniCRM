using Microsoft.AspNetCore.Mvc;
using MiniCRM.Services;
using MiniCRM.ViewModels;

namespace MiniCRM.Controllers
{
    public class RolePermissionController : Controller
    {
        private readonly IRolePermissionService
            _rolePermissionService;

        private readonly IAuthorizationService
            _authorizationService;

        private readonly IUserActivityLogService _activityLogService;


        public RolePermissionController(
            IRolePermissionService rolePermissionService,
            IAuthorizationService authorizationService,
            IUserActivityLogService activityLogService)
        {
            _rolePermissionService =
                rolePermissionService;

            _authorizationService =
                authorizationService;

            _activityLogService = activityLogService;
        }


        [HttpGet]
        public IActionResult Edit(int id)
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
                _rolePermissionService
                    .GetRolePermissionEditModel(id);

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(
            RolePermissionEditViewModel model)
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

            model.SelectedPermissionIds ??=
                new List<int>();

            bool result =
                _rolePermissionService
                    .UpdateRolePermissions(
                        model.RoleId,
                        model.SelectedPermissionIds,
                        out string message);

            if (!result)
            {
                ModelState.AddModelError(
                    string.Empty,
                    message);

                var refreshedModel =
                    _rolePermissionService
                        .GetRolePermissionEditModel(
                            model.RoleId);

                if (refreshedModel == null)
                {
                    return NotFound();
                }

                refreshedModel.SelectedPermissionIds =
                    model.SelectedPermissionIds;

                return View(refreshedModel);
            }

            TempData["SuccessMessage"] =
                message;

            return RedirectToAction(
                "Index",
                "User");
        }
        [HttpGet]
        public IActionResult Index()
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
                "Role.Manage"))
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden);
            }

            var roles =
                _rolePermissionService.GetAllRoles();

            return View(roles);
        }

        [HttpGet]
        public IActionResult Create()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!_authorizationService.HasPermission(userId.Value, "Role.Manage"))
            {
                return StatusCode(StatusCodes.Status403Forbidden);
            }

            return View(new RoleCreateViewModel());
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(RoleCreateViewModel model)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (!userId.HasValue)
            {
                return RedirectToAction("Login", " Auth");
            }
            if (!_authorizationService.HasPermission(userId.Value, "Role.Manage"))
            {
                return StatusCode(StatusCodes.Status403Forbidden);
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            bool result = _rolePermissionService.CreateRole(model, out int roleId, out string message);

            if (!result)
            {
                ModelState.AddModelError(string.Empty, message);
                return View(model);
            }
            _activityLogService.Log(
                userId.Value,
                "RoleCreated",
                $"{model.Name} rolü oluşturuldu.");

            TempData["SuccessMessage"] = message;

            return RedirectToAction(nameof(Edit), new
            {
                id = roleId
            });
        }
    }
}