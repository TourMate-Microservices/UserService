using Grpc.Core;
using TourMate.UserService.Api.Grpc;
using TourMate.UserService.Services.IServices;

namespace TourMate.UserService.Api.Services
{
    public class UserGrpcService : Grpc.UserService.UserServiceBase
    {
        private readonly IAccountService _accountService;
        private readonly ILogger<UserGrpcService> _logger;

        public UserGrpcService(IAccountService accountService, ILogger<UserGrpcService> logger)
        {
            _accountService = accountService;
            _logger = logger;
        }

        public override async Task<User> GetUser(GetUserByIdRequest request, ServerCallContext context)
        {
            try
            {
                _logger.LogInformation("Getting user with ID: {UserId}", request.Id);

                var account = await _accountService.GetAccountByIdAsync(request.Id);
                
                if (account == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found", request.Id);
                    throw new RpcException(new Status(StatusCode.NotFound, $"User with ID {request.Id} not found"));
                }

                var grpcUser = new User
                {
                    Id = account.AccountId,
                    Fullname = account.Email, // Use email as fullname for now
                    Email = account.Email
                };

                _logger.LogInformation("Successfully retrieved user: {UserEmail}", account.Email);
                return grpcUser;
            }
            catch (RpcException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user with ID: {UserId}", request.Id);
                throw new RpcException(new Status(StatusCode.Internal, "Internal server error"));
            }
        }
    }
}
