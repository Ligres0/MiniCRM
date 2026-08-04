using MiniCRM.Models;
using System.ComponentModel.DataAnnotations;

namespace MiniCRM.ViewModels
{
    public class ProductFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Product name is required.")]
        [StringLength(
            150,
            ErrorMessage = "Product name cannot exceed 150 characters.")]
        public string Name { get; set; } = string.Empty;

        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Please select a category.")]
        public int CategoryId { get; set; }

        [Range(
            0,
            double.MaxValue,
            ErrorMessage = "Price cannot be negative.")]
        public decimal Price { get; set; }

        [Range(
            0,
            int.MaxValue,
            ErrorMessage = "Stock quantity cannot be negative.")]
        public int Stock { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; }

        // Categories for the dropdown list
        public List<Category> Categories { get; set; } = new();
    }
}