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
    public class AccountRepository : IAccountRepository
    {
        private TourMateUserContext _context;

        public AccountRepository()
        {
            _context ??= new();
        }

        public AccountRepository(TourMateUserContext context)
        {
            _context = context;
        }

        public async Task<Account> GetAccountByLogin(string email, string password)
        {
            return await _context.Accounts.Include(a => a.Role).FirstOrDefaultAsync(a => a.Email == email && a.Password == password && a.Status);
        }

        public async Task<Account> GetAccountByEmail(string email)
        {
            return await _context.Accounts.Include(a => a.Role).FirstOrDefaultAsync(a => a.Email == email);
        }

        public async Task<Account> GetAccountByIdAsync(int accountId)
        {
            return await _context.Accounts.Include(a => a.Role).FirstOrDefaultAsync(a => a.AccountId == accountId);
        }

        public async Task<Account> CreateAndReturnAsync(Account account)
        {
            try
            {
                _context.Add(account);
                await _context.SaveChangesAsync();
                return account;
            }
            catch
            {
                return account;
            }
        }
    }
}
