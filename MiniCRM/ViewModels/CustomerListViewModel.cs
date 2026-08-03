using MiniCRM.Models;


namespace MiniCRM.ViewModels
{
    public class CustomerListViewModel
    {
        public List<Customers> Customers { get; set; } = new();

        public string? Search { get; set; }
        public string? CompanyName { get; set; }
        public bool? IsActive { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
    }
}
