using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourMate.UserService.Repositories.Models;
using TourMate.UserService.Repositories.RequestModels;

namespace TourMate.UserService.Repositories.IRepositories
{
    public interface IAccountRepository
    {
        Task<Account> GetAccountByLogin(string email, string password);
        Task<Account> GetAccountByEmail(string email);
        Task<Account> GetAccountByIdAsync(int accountId);
        Task<Account> CreateAndReturnAsync(Account account);
    }
}
