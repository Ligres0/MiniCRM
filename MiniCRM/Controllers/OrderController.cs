using Microsoft.AspNetCore.Mvc;
using MiniCRM.Models;
using MiniCRM.Repositories;
using MiniCRM.Services;
using MiniCRM.ViewModels;

namespace MiniCRM.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ICustomerService _customerService;
        private readonly IProductService _productService;
        private readonly IAuthorizationService _authorizationService;

        public OrderController(
            IOrderService orderService,
            ICustomerService customerService,
            IProductService productService,
            IAuthorizationService authorizationService)
        {
            _orderService = orderService;
            _customerService = customerService;
            _productService = productService;
            _authorizationService = authorizationService;
        }

        public IActionResult Index(string? search,
            int? customerId,
            Order.OrderStatus? status,
            DateTime? startDate,
            DateTime? endDate,
            decimal? minAmount,
            decimal? maxAmount,
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
                "Order.View"))
            {
                return Forbid();
            }

            bool filterError = false;
            if (pageNumber < 1)
            {
                pageNumber = 1;
            }
            if (pageSize <= 0)
            {
                pageSize = 10;
            }
            if(startDate.HasValue && endDate.HasValue && startDate > endDate)
            {
                ModelState.AddModelError(string.Empty, "Start date cannot be greater than end date.");
                filterError = true;
            }
            if(minAmount.HasValue && maxAmount.HasValue && minAmount > maxAmount)
            {
                ModelState.AddModelError(string.Empty, "Minimum amount cannot be greater than maximum amount.");
                filterError = true;
            }

            int totalCount = _orderService.GetFilteredCount(
                search,
                customerId,
                status,
                startDate,
                endDate,
                minAmount,
                maxAmount);


            int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);





            var orders = _orderService.GetFilteredPaged(search, customerId, status,startDate, endDate, minAmount, maxAmount, pageNumber, pageSize);
            var customers =_customerService.GetAllActive();
            if (filterError)
            {
                var errorViewModel = new OrderListViewModel
                {
                    Orders = new List<Order>(),
                    Customers = customers,

                    Search = search,
                    CustomerId = customerId,
                    Status = status,
                    StartDate = startDate,
                    EndDate = endDate,
                    MinAmount = minAmount,
                    MaxAmount = maxAmount,

                    CurrentPage = 1,
                    TotalPages = 0,
                    TotalCount = 0,
                    PageSize = pageSize
                };

                return View(errorViewModel);

            }
            var viewModel = new OrderListViewModel
            {
                Orders = orders,
                Customers = customers,
                Search = search,
                CustomerId = customerId,
                Status = status,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                TotalCount = totalCount,
                PageSize = pageSize,
                StartDate = startDate,
                EndDate = endDate,
                MinAmount = minAmount,
                MaxAmount = maxAmount,
            };
            return View(viewModel);

        }

        [HttpGet]
        public IActionResult Details(int id)
        {

            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!_authorizationService.HasPermission(
                userId.Value,
                "Order.View"))
            {
                return Forbid();
            }

            var order = _orderService.GetById(id);

            if (order == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var details = _orderService.GetDetailsByOrderId(id);
            var auditLogs =_orderService.GetAuditLogs(id);

            var customer = _customerService.GetById(order.CustomerId);

            var items = new List<OrderItemViewModel>();

            foreach (var detail in details)
            {
                var product = _productService.GetById(detail.ProductId);

                items.Add(new OrderItemViewModel
                {
                    ProductId = detail.ProductId,
                    ProductName = product?.Name ?? "Product not found",
                    Quantity = detail.Quantity,
                    UnitPrice = detail.UnitPrice,
                    TotalPrice = detail.TotalPrice
                });
            }

            var viewModel = new OrderDetailsViewModel
            {
                Order = order,

                CustomerName = customer == null
                    ? "Customer not found"
                    : $"{customer.Name} {customer.Surname}",

                Items = items,
                AuditLogs = auditLogs
            };

            return View(viewModel);
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
                "Order.Create"))
            {
                return Forbid();
            }
            var viewModel = new OrderCreateViewModel();

            FillCreateLists(viewModel);

            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(OrderCreateViewModel model)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            if (!_authorizationService.HasPermission(
                userId.Value,
                "Order.Create"))
            {
                return Forbid();
            }

            if (!ModelState.IsValid)
            {
                FillCreateLists(model);
                return View(model);
            }

            

            bool result = _orderService.CreateOrder(
                model,
                userId.Value,
                out string message);

            if (result)
            {
                TempData["SuccessMessage"] = message;

                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, message);

            FillCreateLists(model);

            return View(model);
        }
        private void FillCreateLists(OrderCreateViewModel model)
        {
            model.Customers = _customerService.GetFilteredPaged(
                search: null,
                companyName: null,
                isActive: true,
                pageNumber: 1,
                pageSize: 1000);

            model.Products = _productService.GetFilteredPaged(
                search: null,
                categoryId: null,
                isActive: true,
                pageNumber: 1,
                pageSize: 1000);
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
                "Order.Edit"))
            {
                return Forbid();
            }
            var order = _orderService.GetById(id);

            if (order == null)
            {
                return RedirectToAction(nameof(Index));
            }

            if (order.Status != Order.OrderStatus.Draft)
            {
                TempData["ErrorMessage"] =
                    "Only draft orders can be updated.";

                return RedirectToAction(
                    nameof(Details),
                    new { id });
            }

            var details =
                _orderService.GetDetailsByOrderId(id);

            var model = new OrderCreateViewModel
            {
                Id = order.Id,
                CustomerId = order.CustomerId,

                Items = details.Select(detail =>
                    new OrderItemViewModel
                    {
                        ProductId = detail.ProductId,
                        Quantity = detail.Quantity,
                        UnitPrice = detail.UnitPrice,
                        TotalPrice = detail.TotalPrice
                    }).ToList()
            };

            FillCreateLists(model);

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(int id,OrderCreateViewModel model)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!_authorizationService.HasPermission(
                userId.Value,
                "Order.Edit"))
            {
                return Forbid();
            }
            if (!ModelState.IsValid)
            {
                FillCreateLists(model);
                return View(model);
            }

            bool result = _orderService.UpdateOrder(
                id,
                model,
                userId.Value,
                out string message);

            if (result)
            {
                TempData["SuccessMessage"] = message;

                return RedirectToAction(
                    nameof(Details),
                    new { id });
            }

            ModelState.AddModelError(
                string.Empty,
                message);

            FillCreateLists(model);

            return View(model);
        }
        [HttpPost]
        public IActionResult Complete(int id)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!_authorizationService.HasPermission(
                userId.Value,
                "Order.Edit"))
            {
                return Forbid();
            }
            if (_orderService.CompleteOrder(id,userId.Value, out string message))
            {
                TempData["SuccessMessage"] = message;
            }
            else
            {
                TempData["ErrorMessage"] = message;
            }

            return RedirectToAction(nameof(Details), new { id });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Cancel(int id)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!_authorizationService.HasPermission(
                userId.Value,
                "Order.Delete"))
            {
                return Forbid();
            }
            if (_orderService.CancelOrder(id,userId.Value, out string message))
            {
                TempData["SuccessMessage"] = message;
            }
            else
            {
                TempData["ErrorMessage"] = message;
            }

            return RedirectToAction(
                nameof(Details),
                new { id });
        }

        public IActionResult GetProductInfo(int productId)
        {
            var product = _productService.GetById(productId);
            if (product == null)
            {
                return NotFound();
            }
            var productInfo = new
            {
                product.Price,
                product.Stock
            };
            return Json(productInfo);

        }

        public IActionResult GetCustomerInfo(int customerId)
        {
            var customer = _customerService.GetById(customerId);
            if (customer == null)
            {
                return NotFound();
            }
            var customerInfo = new
            {
                customer.Name,
                customer.Surname,
                customer.CompanyName,
                customer.Email,
                customer.Phone
            };
            return Json(customerInfo);
        }




    }
    }

