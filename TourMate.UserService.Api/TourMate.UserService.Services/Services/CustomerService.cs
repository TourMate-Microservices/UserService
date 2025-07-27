using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourMate.UserService.Repositories.IRepositories;
using TourMate.UserService.Repositories.Models;
using TourMate.UserService.Repositories.ResponseModels;
using TourMate.UserService.Services.IServices;

namespace TourMate.UserService.Services.Services
{
    public class CustomerService : ICustomerService
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

        public async Task<Customer> GetCustomerById(int id)
        {
            return await _repository.GetById(id);
        }

        public async Task<Customer> GetCustomerByPhone(string phone)
        {
            return await _repository.GetByPhone(phone);
        }

        public async Task<bool> CreateCustomer(Customer customer)
        {
            return await _repository.CreateAsync(customer);
        }

        public async Task<PagedResult<Customer>> GetPagedCustomersAsync(int pageIndex, int pageSize, string fullName)
        {
            return await _repository.GetPagedCustomer(pageSize, pageIndex, fullName);
        }

        public async Task<Dictionary<int, Customer>> GetCustomersFromIds(HashSet<int> ids)
        {
            return await _repository.GetCustomersFromIds(ids);
        }
    }
}
