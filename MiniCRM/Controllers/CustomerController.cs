using Microsoft.AspNetCore.Mvc;
using MiniCRM.Models;
using MiniCRM.Services;

namespace MiniCRM.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ICustomerService _customerService;
        private readonly IAuthorizationService _authorizationService;

        public CustomerController(ICustomerService customerService, IAuthorizationService authorizationService)
        {
            _customerService = customerService;
            _authorizationService = authorizationService;
        }
        public IActionResult Index(
            string? search,
            string? companyName,
            bool? isActive,
            int pageNumber = 1,
            int pageSize = 10)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!_authorizationService.HasPermission(
                userId.Value,
                "Customer.View"))
            {
                return Forbid();
            }

            var customers = _customerService.GetFilteredPaged(
                search,
                companyName,
                isActive,
                pageNumber,
                pageSize);
            var totalCount = _customerService.GetFilteredCount(
                search,
                companyName,
                isActive);
            var viewModel = new ViewModels.CustomerListViewModel
            {
                Customers = customers,
                Search = search,
                CompanyName = companyName,
                IsActive = isActive,
                CurrentPage = pageNumber,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                TotalCount = totalCount,
                PageSize = pageSize
            };
            return View(viewModel);
        }

        public IActionResult Details(int id)
        {
            var customer = _customerService.GetById(id);

            if (customer == null)
            {
                return RedirectToAction("Index");
            }
            return View(customer);
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
                "Customer.Create"))
            {
                return Forbid();
            }

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Customers customer)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!_authorizationService.HasPermission(
                userId.Value,
                "Customer.Create"))
            {
                return Forbid();
            }

            if (!ModelState.IsValid)
            {
                return View(customer);
            }

            bool result = _customerService.Insert(customer, out string message);

            if (result)
            {
                TempData["SuccessMessage"] = message;
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", message);
            return View(customer);
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
                "Customer.Edit"))
            {
                return Forbid();
            }

            var customer = _customerService.GetById(id);

            if (customer == null)
            {
                return RedirectToAction("Index");
            }

            return View(customer);
        }
        [HttpPost]
        public IActionResult Update(Customers customer)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!_authorizationService.HasPermission(
                userId.Value,
                "Customer.Edit"))
            {
                return Forbid();
            }
            if (!ModelState.IsValid)
            {
                return View(customer);
            }

            bool result = _customerService.Update(customer, out string message);
            if (result)
            {
                TempData["SuccessMessage"] = message;
                return RedirectToAction(nameof(Index));

            }
            ModelState.AddModelError("", message);
            return View(customer);
        }

        public IActionResult Deactivate(int id)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!_authorizationService.HasPermission(
                userId.Value,
                "Customer.Delete"))
            {
                return Forbid();
            }

            bool result = _customerService.Deactivate(id, out string message);

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
