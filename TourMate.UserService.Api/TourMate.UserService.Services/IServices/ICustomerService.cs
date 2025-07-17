using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourMate.UserService.Repositories.Models;

namespace TourMate.UserService.Services.IServices
{
    public interface ICustomerService
    {
        Task<Customer> GetCustomerByAccId(int accId);
        Task<bool> CreateCustomer(Customer customer);
        Task<Customer> GetCustomerByPhone(string phone);
    }
}
