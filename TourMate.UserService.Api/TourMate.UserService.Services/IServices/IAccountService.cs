using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourMate.UserService.Repositories.Models;
using TourMate.UserService.Repositories.ResponseModels;

namespace TourMate.UserService.Services.IServices
{
    public interface IAccountService
    {
        Task<Account> GetAccountByEmail(string email);
        Task<Account> GetAccountByIdAsync(int accountId);
        Task<Account> CreateAccount(Account account);
        Task<AuthResponse> LoginAsync(string email, string password);
    }
}
