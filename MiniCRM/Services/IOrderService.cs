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
            DateTime? startDate,
            DateTime? endDate,
            decimal? minAmount,
            decimal? maxAmount,
            int pageNumber,
            int pageSize);

        int GetFilteredCount(
            string? search,
            int? customerId,
            Order.OrderStatus? status,
            DateTime? startDate,
            DateTime? endDate,
            decimal? minAmount,
            decimal? maxAmount);

        Order? GetById(int id);

        List<OrderDetails> GetDetailsByOrderId(int orderId);

        bool CreateOrder(
            OrderCreateViewModel viewModel,
            int createdByUserId,
            out string message);

        bool UpdateOrder(
            int orderId,
            OrderCreateViewModel viewModel,
            int changedByUserId,
            out string message);

        bool CompleteOrder(
            int orderId,
            int changedByUserId,
            out string message);

        bool CancelOrder(
            int orderId,
            int changedByUserId,
            out string message);

        List<OrderStatusHistory> GetStatusHistory(int orderId);
        List<OrderAuditLog> GetAuditLogs(int orderId);
    }
}
