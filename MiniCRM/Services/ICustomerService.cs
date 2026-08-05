using MiniCRM.Models;


namespace MiniCRM.Services
{
    public interface ICustomerService
    {

        List<Customers> GetFilteredPaged(
            string? search,
            string? companyName,
            bool? isActive,
            int pageNumber,
            int pageSize
        );

        int GetFilteredCount(
            string? search,
            string? companyName,
            bool? isActive
        );

        Customers? GetById(int id);
        List<Customers> GetAllActive();

        bool Insert(Customers customer, out string message);

        bool Update(Customers customer, out string message);

        bool Deactivate(int id, out string message);

    }
}
