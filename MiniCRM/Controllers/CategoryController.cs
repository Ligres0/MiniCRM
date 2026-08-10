using Microsoft.AspNetCore.Mvc;
using MiniCRM.Models;
using MiniCRM.Services;

namespace MiniCRM.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IAuthorizationService _authorizationService;

        public CategoryController(
            ICategoryService categoryService,
            IAuthorizationService authorizationService)
        {
            _categoryService = categoryService;
            _authorizationService = authorizationService;
        }

        public IActionResult Index()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!_authorizationService.HasPermission(
                userId.Value,
                "Product.View"))
            {
                return Forbid();
            }
            var categories =
                _categoryService.GetAllCategories();

            return View(categories);
        }

        public IActionResult Details(int id)
        {
            var category =
                _categoryService.GetById(id);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpGet]
        public IActionResult Create()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!_authorizationService.HasPermission(
                userId.Value,
                "Product.Create"))
            {
                return Forbid();
            }
            return View(new Category
            {
                IsActive = true
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!_authorizationService.HasPermission(
                userId.Value,
                "Product.Create"))
            {
                return Forbid();
            }
            if (!ModelState.IsValid)
            {
                return View(category);
            }

            bool result = _categoryService.Insert(
                category,
                out string message);

            if (result)
            {
                TempData["SuccessMessage"] = message;
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, message);

            return View(category);
        }

        [HttpGet]
        public IActionResult Update(int id)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!_authorizationService.HasPermission(
                userId.Value,
                "Product.Edit"))
            {
                return Forbid();
            }
            var category =
                _categoryService.GetById(id);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(Category category)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!_authorizationService.HasPermission(
                userId.Value,
                "Product.Edit"))
            {
                return Forbid();
            }
            if (!ModelState.IsValid)
            {
                return View(category);
            }

            bool result = _categoryService.Update(
                category,
                out string message);

            if (result)
            {
                TempData["SuccessMessage"] = message;
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, message);

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Deactivate(int id)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!_authorizationService.HasPermission(
                userId.Value,
                "Product.Delete"))
            {
                return Forbid();
            }
            bool result = _categoryService.Deactivate(
                id,
                out string message);

            if (result)
            {
                TempData["SuccessMessage"] = message;
            }
            else
            {
                TempData["ErrorMessage"] = message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}