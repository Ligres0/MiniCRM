using MiniCRM.Models;

namespace MiniCRM.Repositories
{
    public interface ICustomerRepository
    {
        Customers? GetById(int id);
        List<Customers> GetAllActive();

        int Insert(Customers customer);
        int Update(Customers customer);
        int Deactivate(int id);

        List<Customers> GetFilteredPaged(
            string? search,
            string? companyName,
            bool? isActive,
            int pageNumber,
            int pageSize
        );
        int GetFilteredCount (
            string? search,
            string? companyName,
            bool? isActive
        );

        bool EmailExists(string email, int? excludeId = null);

    }
}
