using MiniCRM.Models;
using MiniCRM.ViewModels;

namespace MiniCRM.Services
{
    public interface IOrderService
    {
        List<Order> GetFilteredPaged(
            string? search,
            int? customerId,
            Order.OrderStatus? status,
            int pageNumber,
            int pageSize);

        int GetFilteredCount(
            string? search,
            int? customerId,
            Order.OrderStatus? status);

        Order? GetById(int id);

        List<OrderDetails> GetDetailsByOrderId(int orderId);

        bool CreateOrder(
            OrderCreateViewModel viewModel,
            int createdByUserId,
            out string message);

        bool UpdateOrder(
            int orderId,
            OrderCreateViewModel viewModel,
            out string message);

        bool CompleteOrder(
            int orderId,
            out string message);

        bool CancelOrder(
            int orderId,
            out string message);
    }
}
