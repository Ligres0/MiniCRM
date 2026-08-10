using Microsoft.AspNetCore.Mvc;
using MiniCRM.Models;
using MiniCRM.Services;
using MiniCRM.ViewModels;

namespace MiniCRM.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IAuthorizationService _authorizationService;

        public ProductController(
            IProductService productService,
            ICategoryService categoryService,
            IAuthorizationService authorizationService)
        {
            _productService = productService;
            _categoryService = categoryService;
            _authorizationService = authorizationService;
        }

        public IActionResult Index(
            string? search,
            int? categoryId,
            bool? isActive,
            int pageNumber = 1)
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
            const int pageSize = 10;

            if (pageNumber < 1)
            {
                pageNumber = 1;
            }

            int totalCount = _productService.GetFilteredCount(
                search,
                categoryId,
                isActive);

            int totalPages =
                (int)Math.Ceiling(totalCount / (double)pageSize);

            if (totalPages > 0 && pageNumber > totalPages)
            {
                pageNumber = totalPages;
            }

            var products = _productService.GetFilteredPaged(
                search,
                categoryId,
                isActive,
                pageNumber,
                pageSize);

            var viewModel = new ProductListViewModel
            {
                Products = products,

                Categories = _categoryService.GetAllActive(),

                Search = search,
                CategoryId = categoryId,
                IsActive = isActive,

                CurrentPage = pageNumber,
                TotalPages = totalPages,
                TotalCount = totalCount,
                PageSize = pageSize
            };

            return View(viewModel);
        }

        public IActionResult Details(int id)
        {
            var product = _productService.GetById(id);
            if (product == null)
            {
                return RedirectToAction("Index");
            }
            return View(product);
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

            var viewModel = new ProductFormViewModel
            {
                IsActive = true,
                Categories = _categoryService.GetAllActive()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProductFormViewModel model)
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
                model.Categories = _categoryService.GetAllActive();
                return View(model);
            }

            var product = new Product
            {
                Name = model.Name,
                CategoryId = model.CategoryId,
                Price = model.Price,
                Stock = model.Stock,
                IsActive = model.IsActive
            };

            if (_productService.Insert(product, out string message))
            {
                TempData["SuccessMessage"] = message;
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, message);

            model.Categories = _categoryService.GetAllActive();

            return View(model);
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

            var existingProduct = _productService.GetById(id);

            if (existingProduct == null)
            {
                return NotFound();
            }

            var viewModel = new ProductFormViewModel
            {
                Id = existingProduct.Id,
                Name = existingProduct.Name,
                CategoryId = existingProduct.CategoryId,
                Price = existingProduct.Price,
                Stock = existingProduct.Stock,
                IsActive = existingProduct.IsActive,
                CreatedDate = existingProduct.CreatedDate,
                Categories = _categoryService.GetAllActive()
            };

            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(ProductFormViewModel model)
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
                model.Categories = _categoryService.GetAllActive();
                return View(model);
            }

            var product = new Product
            {
                Id = model.Id,
                Name = model.Name,
                CategoryId = model.CategoryId,
                Price = model.Price,
                Stock = model.Stock,
                IsActive = model.IsActive,
                CreatedDate = model.CreatedDate
            };

            if (_productService.Update(product, out string message))
            {
                TempData["SuccessMessage"] = message;
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, message);

            model.Categories = _categoryService.GetAllActive();

            return View(model);
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
            bool result = _productService.Deactivate(id, out string message);

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
