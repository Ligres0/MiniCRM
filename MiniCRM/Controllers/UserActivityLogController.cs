using Microsoft.AspNetCore.Mvc;
using MiniCRM.Services;

namespace MiniCRM.Controllers
{
    public class UserActivityLogController : Controller
    {
        private readonly IUserActivityLogService
            _activityLogService;

        private readonly IAuthorizationService
            _authorizationService;


        public UserActivityLogController(
            IUserActivityLogService activityLogService,
            IAuthorizationService authorizationService)
        {
            _activityLogService =
                activityLogService;

            _authorizationService =
                authorizationService;
        }


        [HttpGet]
        public IActionResult Index()
        {
            int? userId =
                HttpContext.Session
                    .GetInt32("UserId");

            if (!userId.HasValue)
            {
                return RedirectToAction(
                    "Login",
                    "Auth");
            }


            if (!_authorizationService
                .HasPermission(
                    userId.Value,
                    "User.View"))
            {
                return StatusCode(
                    StatusCodes
                        .Status403Forbidden);
            }


            var logs =
                _activityLogService
                    .GetAll();


            return View(logs);
        }
    }
}