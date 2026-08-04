using MiniCRM.Models;

namespace MiniCRM.ViewModels
{
    public class ProductListViewModel
    {
        public List<Product> Products { get; set; } = new();

        public List<Category> Categories { get; set; } = new();

        public string? Search { get; set; }

        public int? CategoryId { get; set; }

        public bool? IsActive { get; set; }

        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }

        public int TotalCount { get; set; }

        public int PageSize { get; set; }
    }
}