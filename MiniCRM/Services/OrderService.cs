using MiniCRM.Models;
using MiniCRM.Repositories;
using MiniCRM.ViewModels;


namespace MiniCRM.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICustomerService _customerService;
        private readonly IProductService _productService;
        private readonly IDashboardService _dashboardService;


        public OrderService(
            IOrderRepository orderRepository,
            ICustomerService customerService,
            IProductService productService,
            IDashboardService dashboardService)
        {
            _orderRepository = orderRepository;
            _customerService = customerService;
            _productService = productService;
            _dashboardService = dashboardService;
        }


        public List<Order> GetFilteredPaged(
            string? search,
            int? customerId,
            Order.OrderStatus? status,
            DateTime? startDate,
            DateTime? endDate,
            decimal? minAmount,
            decimal? maxAmount,
            int pageNumber,
            int pageSize)
        {
            if (pageNumber < 1)
            {
                pageNumber = 1;
            }

            if (pageSize < 1)
            {
                pageSize = 10;
            }
            if(minAmount  < 0)
            {
                minAmount = null;
            }
            if(maxAmount < 0) 
            {
                maxAmount = null;
            }

            return _orderRepository.GetFilteredPaged(
                search,
                customerId,
                status,
                startDate,
                endDate,
                minAmount,
                maxAmount,
                pageNumber,
                pageSize);
        }

        public int GetFilteredCount(
            string? search,
            int? customerId,
            Order.OrderStatus? status,
            DateTime? startDate,
            DateTime? endDate,
            decimal? minAmount,
            decimal? maxAmount)
        {
            return _orderRepository.GetFilteredCount(
                search,
                customerId,
                status,
                startDate,
                endDate,
                minAmount,
                maxAmount);
        }

        public Order? GetById(int id)
        {
            if (id <= 0)
            {
                return null;
            }

            return _orderRepository.GetById(id);
        }

        public List<OrderDetails> GetDetailsByOrderId(int orderId)
        {
            if (orderId <= 0)
            {
                return new List<OrderDetails>();
            }

            return _orderRepository.GetDetailsByOrderId(orderId);
        }

        public bool CreateOrder(
            OrderCreateViewModel viewModel,
            int createdByUserId,
            out string message)
        {
            var customer = _customerService.GetById(viewModel.CustomerId);

            if(customer == null) //musteri var mi
            {
                message = "Customer not found.";
                return false;
            }
            if(!customer.IsActive) // musteri aktif mi
            {
                message = "Customer is not active.";
                return false;
            }
            if(viewModel.Items == null || viewModel.Items.Count == 0) // sipariste urun var mi
            {
                message = "Order must have at least one item.";
                return false;
            }
            var orderDetails = new List<OrderDetails>();

            decimal totalAmount = 0;
            foreach (var item in viewModel.Items) 
            {
                if(item.Quantity <= 0) 
                {
                    message = "Quantity must be greater than zero.";
                    return false;
                }
                var product = _productService.GetById(item.ProductId);
                if (product == null) // urun var mi
                {
                    message = "One of the selected products was not found."; // urun aktif mi
                    return false;
                }
                if(!product.IsActive) // urun aktif mi
                {
                    message = $"Product {product.Name} is not active.";
                    return false;
                }
                if(item.Quantity > product.Stock)// stok yeterli mi
                {
                    message = $"Not enough stock for product {product.Name}.";
                    return false;
                }

                decimal unitPrice = product.Price;
                decimal lineTotal = item.Quantity * unitPrice;

                orderDetails.Add(new OrderDetails
                {
                    ProductId = product.Id,
                    Quantity = item.Quantity,
                    UnitPrice = unitPrice,
                    TotalPrice = lineTotal
                });

                totalAmount += lineTotal;
            }
            var order = new Order
            {
                CustomerId = viewModel.CustomerId,
                CreatedByUserId = createdByUserId,
                OrderDate = DateTime.Now,
                TotalAmount = totalAmount,
                Status = Order.OrderStatus.Draft
            };

            try
            {
                int orderId =
                    _orderRepository.CreateOrderTransaction(
                        order,
                        orderDetails,
                        createdByUserId);

                if (orderId <= 0)
                {
                    message = "Order could not be created.";
                    return false;
                }
                var auditLog = new OrderAuditLog
                {
                    OrderId = orderId,
                    ActionType = "Created",
                    Description = "Order created as Draft.",
                    ChangedByUserId = createdByUserId,
                    ChangedAt = DateTime.Now
                };

                _orderRepository.InsertAuditLog(auditLog);

                _dashboardService.ClearDashboardCache();


                message = "Order created successfully.";
                return true;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return false;
            }
        }
        public bool UpdateOrder(
    int orderId,
    OrderCreateViewModel viewModel,
    int changedByUserId,
    out string message)
        {
            var existingOrder =
                _orderRepository.GetById(orderId);

            if (existingOrder == null)
            {
                message = "Order not found.";
                return false;
            }

            if (existingOrder.Status != Order.OrderStatus.Draft)
            {
                message = "Only draft orders can be updated.";
                return false;
            }

            if (viewModel.Items == null ||
                viewModel.Items.Count == 0)
            {
                message = "Order must have at least one item.";
                return false;
            }
            var oldDetails = _orderRepository.GetDetailsByOrderId(orderId);

            decimal oldTotalAmount =
                existingOrder.TotalAmount;

            var newDetails = new List<OrderDetails>();
            decimal totalAmount = 0;

            foreach (var item in viewModel.Items)
            {
                if (item.Quantity <= 0)
                {
                    message = "Quantity must be greater than zero.";
                    return false;
                }

                var product =
                    _productService.GetById(item.ProductId);

                if (product == null)
                {
                    message = "One of the selected products was not found.";
                    return false;
                }

                if (!product.IsActive)
                {
                    message = $"Product {product.Name} is not active.";
                    return false;
                }

                if (item.Quantity > product.Stock)
                {
                    message = $"Not enough stock for product {product.Name}.";
                    return false;
                }

                decimal unitPrice = product.Price;
                decimal lineTotal = item.Quantity * unitPrice;

                newDetails.Add(new OrderDetails
                {
                    OrderId = orderId,
                    ProductId = product.Id,
                    Quantity = item.Quantity,
                    UnitPrice = unitPrice,
                    TotalPrice = lineTotal
                });

                totalAmount += lineTotal;
            }

            existingOrder.TotalAmount = totalAmount;

            // Müşteri, oluşturulma tarihi ve oluşturan kullanıcı değişmiyor.
            int updatedRows =
                _orderRepository.Update(existingOrder);

            if (updatedRows == 0)
            {
                message = "Order could not be updated.";
                return false;
            }

            _orderRepository.DeleteDetailsByOrderId(orderId);

            foreach (var detail in newDetails)
            {
                int detailId =
                    _orderRepository.InsertOrderDetail(detail);

                if (detailId <= 0)
                {
                    message = "Order details could not be updated.";
                    return false;
                }
            }
            var changes = new List<string>();

            foreach (var newDetail in newDetails)
            {
                var oldDetail =
                    oldDetails.FirstOrDefault(
                        x => x.ProductId == newDetail.ProductId);

                var product =
                    _productService.GetById(
                        newDetail.ProductId);

                string productName =
                    product?.Name ?? $"Product #{newDetail.ProductId}";


                // Önceden siparişte olmayan ürün eklenmiş
                if (oldDetail == null)
                {
                    changes.Add(
                        $"{productName} added to order " +
                        $"(Quantity: {newDetail.Quantity}).");

                    continue;
                }


                // Ürünün miktarı değiştirilmiş
                if (oldDetail.Quantity != newDetail.Quantity)
                {
                    changes.Add(
                        $"{productName} quantity changed " +
                        $"from {oldDetail.Quantity} " +
                        $"to {newDetail.Quantity}.");
                }
            }
            foreach (var oldDetail in oldDetails)
            {
                bool stillExists =
                    newDetails.Any(
                        x => x.ProductId == oldDetail.ProductId);

                if (!stillExists)
                {
                    var product =
                        _productService.GetById(
                            oldDetail.ProductId);

                    string productName =
                        product?.Name ?? $"Product #{oldDetail.ProductId}";

                    changes.Add(
                        $"{productName} removed from order " +
                        $"(Previous quantity: {oldDetail.Quantity}).");
                }
            }
            if (oldTotalAmount != totalAmount)
            {
                changes.Add(
                    $"Total amount changed from " +
                    $"{oldTotalAmount:N2} TL to " +
                    $"{totalAmount:N2} TL.");
            }
            if (changes.Any())
            {
                var auditLog = new OrderAuditLog
                {
                    OrderId = orderId,
                    ActionType = "Updated",

                    Description =
                        string.Join(" ", changes),

                    ChangedByUserId = changedByUserId,

                    ChangedAt = DateTime.Now
                };

                _orderRepository.InsertAuditLog(
                    auditLog);
            }
            _dashboardService.ClearDashboardCache();


            message = "Order updated successfully.";
            return true;
        }

        public bool CompleteOrder(
            int orderId,
            int changedByUserId,
            out string message)
        {
            var order =
                _orderRepository.GetById(orderId);

            if (order == null)
            {
                message = "Order not found.";
                return false;
            }

            if (order.Status !=
                Order.OrderStatus.Draft)
            {
                message =
                    "Only draft orders can be completed.";

                return false;
            }

            var details =
                _orderRepository.GetDetailsByOrderId(orderId);

            if (details.Count == 0)
            {
                message =
                    "An empty order cannot be completed.";

                return false;
            }

            foreach (var detail in details)
            {
                var product =
                    _productService.GetById(
                        detail.ProductId);

                if (product == null)
                {
                    message = "Product not found.";
                    return false;
                }

                if (!product.IsActive)
                {
                    message =
                        $"The product '{product.Name}' is inactive.";

                    return false;
                }

                if (detail.Quantity > product.Stock)
                {
                    message =
                        $"Insufficient stock for '{product.Name}'.";

                    return false;
                }
            }

            int affectedRows =
                _orderRepository.UpdateStatus(
                    orderId,
                    Order.OrderStatus.Completed);

            if (affectedRows == 0)
            {
                message = "Order could not be completed.";
                return false;
            }

            var auditLog = new OrderAuditLog
            {
                OrderId = orderId,
                ActionType = "Completed",
                Description = "Order status changed from Draft to Completed.",
                ChangedByUserId = changedByUserId,
                ChangedAt = DateTime.Now
            };

            _orderRepository.InsertAuditLog(auditLog);

            _dashboardService.ClearDashboardCache();

            message = "Order completed successfully.";
            return true;


            
        }
        public bool CancelOrder(
            int orderId,
            int changedByUserId,
            out string message)
        {
            var order =
                _orderRepository.GetById(orderId);

            if (order == null)
            {
                message = "Order not found.";
                return false;
            }

            if (order.Status !=
                Order.OrderStatus.Draft)
            {
                message =
                    "Only draft orders can be cancelled.";

                return false;
            }

            try
            {
                int result =
                    _orderRepository.CancelOrderTransaction(
                        orderId,
                        order.CreatedByUserId);

                if (result <= 0)
                {
                    message = "Order could not be cancelled.";
                    return false;
                }
                var auditLog = new OrderAuditLog
                {
                    OrderId = orderId,
                    ActionType = "Cancelled",
                    Description = "Order status changed from Draft to Cancelled.",
                    ChangedByUserId = changedByUserId,
                    ChangedAt = DateTime.Now
                };

                _orderRepository.InsertAuditLog(auditLog);

                _dashboardService.ClearDashboardCache();

                message = "Order cancelled successfully.";
                return true;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return false;
            }
        }

        public List<OrderStatusHistory> GetStatusHistory(int orderId)
        {
            return _orderRepository.GetStatusHistory(orderId);
        }
        public List<OrderAuditLog> GetAuditLogs(int orderId)
        {
            if (orderId <= 0)
            {
                return new List<OrderAuditLog>();
            }

            return _orderRepository.GetAuditLogs(orderId);
        }


    }
}