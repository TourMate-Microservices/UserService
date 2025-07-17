using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourMate.UserService.Repositories.Context;
using TourMate.UserService.Repositories.IRepositories;
using TourMate.UserService.Repositories.Models;

namespace TourMate.UserService.Repositories.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private TourMateUserContext _context;

        public CustomerRepository()
        {
            _context ??= new();
        }

        public CustomerRepository(TourMateUserContext context)
        {
            _context = context;
        }

        public async Task<Customer> GetByAccId(int accId)
        {
            return await _context.Customers.FirstOrDefaultAsync(x => x.AccountId == accId);
        }

        public async Task<Customer> GetByPhone(string phone)
        {
            return await _context.Customers.FirstOrDefaultAsync(x => x.Phone == phone);
        }

        public async Task<bool> CreateAsync(Customer customer)
        {
            try
            {
                _context.Add(customer);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
