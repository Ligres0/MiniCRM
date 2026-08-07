using MiniCRM.Models;

namespace MiniCRM.Repositories
{
    public interface IOrderRepository
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
        int InsertOrder(Order order);
        int InsertOrderDetail(OrderDetails detail);
        int Update(Order order);
        int UpdateStatus(int orderId, Order.OrderStatus status);
        int DeleteDetailsByOrderId(int orderId);

        int CreateOrderTransaction(Order order, List<OrderDetails> details, int createdByUserId);

        int CancelOrderTransaction(int orderId, int createdByUserId);
    }
}
