using MiniCRM.Models;

namespace MiniCRM.Repositories
{
    public interface ICategoryRepository
    {
        List<Category> GetAllCategories();
        Category? GetById(int id);
        int Insert(Category category);
        int Update(Category category);
        int Deactivate(int id);
        bool CategoryNameExists(string name, int? excludeId = null);


    }
}
