using MiniCRM.Models;
using System.ComponentModel.DataAnnotations;

namespace MiniCRM.ViewModels
{
    public class OrderItemViewModel
    {
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Please select a product.")]
        public int ProductId { get; set; }

        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Quantity must be greater than zero.")]
        public int Quantity { get; set; }

       
        public decimal UnitPrice { get; set; }

        public decimal TotalPrice { get; set; }

       
        public string? ProductName { get; set; }
    }
}
