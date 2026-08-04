using MiniCRM.Models;

namespace MiniCRM.Services
{
    public interface ICategoryService
    {
        List<Category> GetAllCategories();

        List<Category> GetAllActive();

        Category? GetById(int id);

        bool Insert(Category category, out string message);

        bool Update(Category category, out string message);

        bool Deactivate(int id, out string message);
    }
}