using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourMate.UserService.Repositories.Models;

namespace TourMate.UserService.Repositories.IRepositories
{
    public interface ICustomerRepository
    {
        Task<Customer> GetByAccId(int accId);
        Task<Customer> GetByPhone(string phone);
        Task<bool> CreateAsync(Customer customer);
    }
}
