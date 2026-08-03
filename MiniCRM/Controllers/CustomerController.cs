using Microsoft.AspNetCore.Mvc;
using MiniCRM.Models;
using MiniCRM.Services;

namespace MiniCRM.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }
        public IActionResult Index(
            string? search,
            string? companyName,
            bool? isActive,
            int pageNumber = 1,
            int pageSize = 10)
        {
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
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Customers customer)
        {
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
            var customer = _customerService.GetById(id);

            if (customer == null)
            {
                return RedirectToAction("Index");
            }

            return View(customer);
        }

        public IActionResult Update(Customers customer)
        {
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
