using MiniCRM.Models;

namespace MiniCRM.Services
{
    public interface IProductService
    {
        List<Product> GetFilteredPaged(
            string? search,
            int? categoryId,
            bool? isActive,
            int pageNumber,
            int pageSize
        );

        int GetFilteredCount(
            string? search,
            int? categoryId,
            bool? isActive
        );

        Product? GetById(int id);

        bool Insert(Product product, out string message);

        bool Update(Product product, out string message);

        bool Deactivate(int id, out string message);
        

    }
}
