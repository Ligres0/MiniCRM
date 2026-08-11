using MiniCRM.Models;
using MiniCRM.Repositories;


namespace MiniCRM.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IDashboardService _dashboardService;

        public ProductService(IProductRepository productRepository,IDashboardService dashboardService)
        {
            _productRepository = productRepository;
            _dashboardService = dashboardService;
        }

        public List<Product> GetFilteredPaged(
            string? search,
            int? categoryId,
            bool? isActive,
            int pageNumber,
            int pageSize)
        {
            if (pageNumber < 1)
            {
                pageNumber = 1;
            }
            if (pageSize <= 0)
            {
                pageSize = 10;
            }
            return _productRepository.GetFilteredPaged(
                search,
                categoryId,
                isActive,
                pageNumber,
                pageSize);
        }

        public int GetFilteredCount(
            string? search,
            int? categoryId,
            bool? isActive)
        {
            return _productRepository.GetFilteredCount(
                search,
                categoryId,
                isActive);
        }

        public Product? GetById(int id)
        {
            if (id <= 0)
            {
                return null;
            }
            return _productRepository.GetById(id);
        }

        public bool Insert(Product product, out string message)
        {
            if (_productRepository.NameExists(product.Name))
            {
                message = "Product name already exists.";
                return false;
            }
            _productRepository.Insert(product);
            _dashboardService.ClearDashboardCache();

            message = "Product inserted successfully.";
            return true;
        }
        public bool Update(Product product, out string message)
        {
            if (_productRepository.NameExists(product.Name, product.Id))
            {
                message = "Product name already exists.";
                return false;
            }
            _productRepository.Update(product);
            _dashboardService.ClearDashboardCache();

            message = "Product updated successfully.";
            return true;
        }
        public bool Deactivate(int id, out string message)
        {
            if (id <= 0)
            {
                message = "Invalid product ID.";
                return false;
            }
            _productRepository.Deactivate(id);
            _dashboardService.ClearDashboardCache();

            message = "Product deactivated successfully.";
            return true;
        }


    }
}
