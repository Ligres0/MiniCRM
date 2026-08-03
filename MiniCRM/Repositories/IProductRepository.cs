using MiniCRM.Models;

namespace MiniCRM.Repositories
{
    public interface IProductRepository
    {
        List<Product> GetFilteredPaged(
            string? search,
            int? categoryId,
            bool? isActive,
            int pageNumber,
            int pageSize);

        int GetFilteredCount(
            string? search,
            int? categoryId,
            bool? isActive);

        Product? GetById(int id);

        int Insert(Product product);

        int Update(Product product);

        int Deactivate(int id);

        bool NameExists(
            string name,
            int? excludeProductId = null);
    }
}