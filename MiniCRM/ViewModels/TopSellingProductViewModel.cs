

namespace MiniCRM.ViewModels
{
    public class TopSellingProductViewModel
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public int TotalSoldQuantity { get; set; }
    }
}
