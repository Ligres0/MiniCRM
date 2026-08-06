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

        public OrderController(
            IOrderService orderService,
            ICustomerService customerService,
            IProductService productService)
        {
            _orderService = orderService;
            _customerService = customerService;
            _productService = productService;
        }

        public IActionResult Index(string? search,
            int? customerId,
            Order.OrderStatus? status,
            int pageNumber = 1,
            int pageSize = 10)
        {
            if (pageNumber < 1)
            {
                pageNumber = 1;
            }
            if (pageSize <= 0)
            {
                pageSize = 10;
            }

            int totalCount = _orderService.GetFilteredCount(
                search,
                customerId,
                status);


            int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);





            var orders = _orderService.GetFilteredPaged(search, customerId, status, pageNumber, pageSize);
            var customers =_customerService.GetAllActive();

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
                PageSize = pageSize
            };
            return View(viewModel);



        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var order = _orderService.GetById(id);

            if (order == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var details = _orderService.GetDetailsByOrderId(id);

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

                Items = items
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var viewModel = new OrderCreateViewModel();

            FillCreateLists(viewModel);

            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(OrderCreateViewModel model)
        {
            
            if (!ModelState.IsValid)
            {
                FillCreateLists(model);
                return View(model);
            }

            // Giriş sistemi henüz olmadığı için geçici kullanıcı Id'si.
            int createdByUserId = 1;

            bool result = _orderService.CreateOrder(
                model,
                createdByUserId,
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
            if (!ModelState.IsValid)
            {
                FillCreateLists(model);
                return View(model);
            }

            bool result = _orderService.UpdateOrder(
                id,
                model,
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
            if (_orderService.CompleteOrder(id, out string message))
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
            if (_orderService.CancelOrder(id, out string message))
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

