using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourMate.UserService.Repositories.IRepositories;
using TourMate.UserService.Repositories.Models;
using TourMate.UserService.Repositories.ResponseModels;
using TourMate.UserService.Services.IServices;
using TourMate.UserService.Services.Utils;

namespace TourMate.UserService.Services.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _repository;
        private readonly TokenService _tokenService;
        private readonly ICustomerService _customerService;
        private readonly ITourGuideService _tourGuideService;

        public AccountService(IAccountRepository repo, TokenService tokenService, ICustomerService customerService, ITourGuideService tourGuideService)
        {
            _repository = repo;
            _tokenService = tokenService;
            _customerService = customerService;
            _tourGuideService = tourGuideService;
        }

        public async Task<Account> GetAccountByEmail(string email)
        {
            // Kiểm tra tài khoản đã tồn tại
            return await _repository.GetAccountByEmail(email);
        }

        public async Task<Account> CreateAccount(Account account)
        {
            // Gọi phương thức bất đồng bộ để tạo tài khoản
            return await _repository.CreateAndReturnAsync(account);
        }

        public async Task<AuthResponse> LoginAsync(string email, string password)
        {
            password = HashString.ToHashString(password);
            var user = await _repository.GetAccountByLogin(email, password);
            if (user == null || user.Password != password)
                return null;

            if (user.Role.RoleName == "Customer")
            {
                var customer = await _customerService.GetCustomerByAccId(user.AccountId);
                var accessToken = _tokenService.GenerateAccessToken(user.AccountId, customer.FullName, "Customer");

                return new AuthResponse
                {
                    AccessToken = accessToken,
                };
            }

            if (user.Role.RoleName == "TourGuide")
            {
                var tourGuide = await _tourGuideService.GetTourGuideByAccId(user.AccountId);
                var accessToken = _tokenService.GenerateAccessToken(user.AccountId, tourGuide.FullName, "TourGuide");

                return new AuthResponse
                {
                    AccessToken = accessToken,
                };
            }

            if (user.Role.RoleName == "Admin")
            {
                var accessToken = _tokenService.GenerateAccessToken(user.AccountId, "Admin", "Admin");

                return new AuthResponse
                {
                    AccessToken = accessToken,
                };
            }

            return null;
        }
    }
}
