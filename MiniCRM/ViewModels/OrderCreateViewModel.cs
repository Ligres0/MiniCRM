using MiniCRM.Models;
using System.ComponentModel.DataAnnotations;

namespace MiniCRM.ViewModels
{
    public class OrderCreateViewModel
    {
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Please select a customer.")]
        public int CustomerId { get; set; }

        [MinLength(
            1,
            ErrorMessage = "An order must contain at least one product.")]
        public List<OrderItemViewModel> Items { get; set; } = new();

        // Müşteri dropdown'u için.
        public List<Customers> Customers { get; set; } = new();

        // Ürün dropdown'u için.
        public List<Product> Products { get; set; } = new();

        // Ekranda genel toplamı göstermek için.
        public decimal TotalAmount { get; set; }

        public int Id { get; set; }
        public Order.OrderStatus Status { get; set; }
    }
}