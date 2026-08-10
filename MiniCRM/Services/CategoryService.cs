using MiniCRM.Models;
using MiniCRM.Repositories;
using Microsoft.Extensions.Caching.Memory;

namespace MiniCRM.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMemoryCache _cache;

        public CategoryService(
            ICategoryRepository categoryRepository, IMemoryCache cache)
        {
            _categoryRepository = categoryRepository;
            _cache = cache;
        }

        public List<Category> GetAllCategories()
        {
            string cacheKey = "Categories:All";
            if (!_cache.TryGetValue(cacheKey, out List<Category>? categories))
            {
                categories =  _categoryRepository.GetAllCategories();

                _cache.Set(cacheKey, categories, TimeSpan.FromMinutes(20));
            }
            return categories ?? new List<Category>();
        
        }

        public List<Category> GetAllActive()
        {
            string cacheKey = "Categories:Active";
            if(!_cache.TryGetValue(cacheKey,
                out List<Category>? categories))
            {
                categories = _categoryRepository.GetAllActive();
                _cache.Set(cacheKey, categories,TimeSpan.FromMinutes(20));
            }
            return categories ?? new List<Category>();
        }

        public Category? GetById(int id)
        {
            if (id <= 0)
            {
                return null;
            }

            return _categoryRepository.GetById(id);
        }

        public bool Insert(
            Category category,
            out string message)
        {
            string normalizedName = category.Name.Trim();

            if (_categoryRepository.CategoryNameExists(normalizedName))
            {
                message = "A category with this name already exists.";
                return false;
            }

            category.Name = normalizedName;

            int affectedRows =
                _categoryRepository.Insert(category);

            if (affectedRows == 0)
            {
                message = "Category could not be created.";
                return false;
            }
            ClearCategoryCache();

            message = "Category created successfully.";
            return true;
        }

        public bool Update(
            Category category,
            out string message)
        {
            var existingCategory =
                _categoryRepository.GetById(category.Id);

            if (existingCategory == null)
            {
                message = "Category not found.";
                return false;
            }

            string normalizedName = category.Name.Trim();

            if (_categoryRepository.CategoryNameExists(
                    normalizedName,
                    category.Id))
            {
                message = "A category with this name already exists.";
                return false;
            }

            category.Name = normalizedName;

            int affectedRows =
                _categoryRepository.Update(category);

            if (affectedRows == 0)
            {
                message = "Category could not be updated.";
                return false;
            }
            ClearCategoryCache();

            message = "Category updated successfully.";
            return true;
        }

        public bool Deactivate(
            int id,
            out string message)
        {
            var category = _categoryRepository.GetById(id);

            if (category == null)
            {
                message = "Category not found.";
                return false;
            }

            if (!category.IsActive)
            {
                message = "Category is already inactive.";
                return false;
            }

            int affectedRows =
                _categoryRepository.Deactivate(id);

            if (affectedRows == 0)
            {
                message = "Category could not be deactivated.";
                return false;
            }
            ClearCategoryCache();

            message = "Category deactivated successfully.";
            return true;
        }
        private void ClearCategoryCache()
        {
            _cache.Remove("Categories:All");
            _cache.Remove("Categories:Active");
        }
    }
}