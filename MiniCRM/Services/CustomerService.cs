using MiniCRM.Models;
using MiniCRM.Repositories;

namespace MiniCRM.Services
{
    public class CustomerService : ICustomerService
    {

        private readonly ICustomerRepository _customerRepository;
        private readonly IDashboardService _dashboardService;

        public CustomerService(ICustomerRepository customerRepository, IDashboardService dashboardService)
        {
            _customerRepository = customerRepository;
            _dashboardService = dashboardService;
        }
        public List<Customers> GetAllActive()
        {
            return _customerRepository.GetAllActive();
        }

        public List<Customers> GetFilteredPaged(
            string? search,
            string? companyName,
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

            return _customerRepository.GetFilteredPaged(
                search,
                companyName,
                isActive,
                pageNumber,
                pageSize);
        }


        public int GetFilteredCount(
            string? search,
            string? companyName,
            bool? isActive)
        {
            return _customerRepository.GetFilteredCount(
                search,
                companyName,
                isActive);
        }

        public Customers? GetById(int id)
        {

            if (id <= 0)
            {
                return null;
            }

            return _customerRepository.GetById(id);

        }

        public bool Insert(Customers customer, out string message)
        {
            if (_customerRepository.EmailExists(customer.Email))
            {
                message = "Email already exists.";
                return false;
            }

            customer.IsActive = true;
            int rowsAffected = _customerRepository.Insert(customer);
            if (rowsAffected > 0)
            {
                _dashboardService.ClearDashboardCache();

                message =
                    "Customer inserted successfully.";

                return true;
            }

            message =
                "Failed to insert customer.";

            return false;

        }

        public bool Update(Customers customer, out string message)
        {
            if (_customerRepository.EmailExists(customer.Email, customer.Id))
            {
                message = "Email already exists.";
                return false;
            }
            int rowsAffected = _customerRepository.Update(customer);
            if (rowsAffected > 0)
            {
                _dashboardService.ClearDashboardCache();

                message =
                    "Customer updated successfully.";

                return true;
            }

            message =
                "Failed to update customer.";

            return false;
        }

        public bool Deactivate(int id, out string message)
        {
            int rowsAffected = _customerRepository.Deactivate(id);
            if (rowsAffected > 0)
            {
                _dashboardService.ClearDashboardCache();

                message =
                    "Customer deactivated successfully.";

                return true;
            }

            message =
                "Failed to deactivate customer.";

            return false;
        }
    }

}
