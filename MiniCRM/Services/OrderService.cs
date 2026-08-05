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

        public OrderService(
            IOrderRepository orderRepository,
            ICustomerService customerService,
            IProductService productService)
        {
            _orderRepository = orderRepository;
            _customerService = customerService;
            _productService = productService;
        }


        public List<Order> GetFilteredPaged(
            string? search,
            int? customerId,
            Order.OrderStatus? status,
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

            return _orderRepository.GetFilteredPaged(
                search,
                customerId,
                status,
                pageNumber,
                pageSize);
        }

        public int GetFilteredCount(
            string? search,
            int? customerId,
            Order.OrderStatus? status)
        {
            return _orderRepository.GetFilteredCount(
                search,
                customerId,
                status);
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

            message = "Order updated successfully.";
            return true;
        }

        public bool CompleteOrder(
            int orderId,
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

                message = "Order cancelled successfully.";
                return true;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return false;
            }
        }
        public bool CancelOrder(
            int orderId,
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

            int affectedRows =
                _orderRepository.UpdateStatus(
                    orderId,
                    Order.OrderStatus.Cancelled);

            if (affectedRows == 0)
            {
                message = "Order could not be cancelled.";
                return false;
            }

            message = "Order cancelled successfully.";
            return true;
        }


    }
}