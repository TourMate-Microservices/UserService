using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourMate.UserService.Repositories.IRepositories;
using TourMate.UserService.Repositories.Models;

namespace TourMate.UserService.Services.Services
{
    public class CustomerService
    {
        private readonly ICustomerRepository _repository;

        public CustomerService(ICustomerRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<Customer> GetCustomerByAccId(int accId)
        {
            return await _repository.GetByAccId(accId);
        }

        public async Task<Customer> GetCustomerByPhone(string phone)
        {
            return await _repository.GetByPhone(phone);
        }

        public async Task<bool> CreateCustomer(Customer customer)
        {
            return await _repository.CreateAsync(customer);
        }
    }
}
