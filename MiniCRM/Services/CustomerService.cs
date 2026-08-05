using MiniCRM.Models;
using MiniCRM.Repositories;

namespace MiniCRM.Services
{
    public class CustomerService : ICustomerService
    {

        private readonly ICustomerRepository _customerRepository;

        public CustomerService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
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
            message = rowsAffected > 0 ? "Customer inserted successfully." : "Failed to insert customer.";
            return rowsAffected > 0;
        }

        public bool Update(Customers customer, out string message)
        {
            if (_customerRepository.EmailExists(customer.Email, customer.Id))
            {
                message = "Email already exists.";
                return false;
            }
            int rowsAffected = _customerRepository.Update(customer);
            message = rowsAffected > 0 ? "Customer updated successfully." : "Failed to update customer.";
            return rowsAffected > 0;
        }

        public bool Deactivate(int id, out string message)
        {
            int rowsAffected = _customerRepository.Deactivate(id);
            message = rowsAffected > 0 ? "Customer deactivated successfully." : "Failed to deactivate customer.";
            return rowsAffected > 0;
        }
    }

}
