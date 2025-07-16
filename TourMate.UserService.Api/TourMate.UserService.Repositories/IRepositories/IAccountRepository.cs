using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourMate.UserService.Repositories.Models;

namespace TourMate.UserService.Repositories.IRepositories
{
    public interface IAccountRepository
    {
        Task<Account> GetAccountByLogin(string email, string password);
    }
}
