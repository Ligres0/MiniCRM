using MiniCRM.Models;
using MiniCRM.Repositories;

namespace MiniCRM.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(
            ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public List<Category> GetAllCategories()
        {
            return _categoryRepository.GetAllCategories();
        }

        public List<Category> GetAllActive()
        {
            return _categoryRepository.GetAllActive();
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

            message = "Category deactivated successfully.";
            return true;
        }
    }
}