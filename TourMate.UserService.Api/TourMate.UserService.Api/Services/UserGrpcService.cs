using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using TourMate.UserService.Repositories.Models;
using TourMate.UserService.Services.IServices;
using TourMate.UserService.Services.Services;
using userservice;

namespace TourMate.UserService.Api.Services
{
    public class UserGrpcService : userservice.UserService.UserServiceBase
    {
        private readonly IAccountService _accountService;
        private readonly ITourGuideService _tourGuideService;
        private readonly ICustomerService _customerService;
        private readonly ILogger<UserGrpcService> _logger;

        public UserGrpcService(IAccountService accountService, ITourGuideService tourGuideService, ILogger<UserGrpcService> logger, ICustomerService customerService)
        {
            _accountService = accountService;
            _tourGuideService = tourGuideService;
            _logger = logger;
            _customerService = customerService;
        }

        public override async Task<SenderRoleResponse> GetSenderRole( SenderIdRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Getting sender role for senderId: {SenderId}", request.SenderId);
            var account = await _accountService.GetAccountByIdAsync(request.SenderId);
            if (account == null)
            {
                _logger.LogWarning("Account with ID {SenderId} not found", request.SenderId);
                throw new RpcException(new Status(StatusCode.NotFound, $"Account with ID {request.SenderId} not found"));
            }
            var response = new SenderRoleResponse
            {
                RoleId = account.Role.RoleId,
                SenderId = account.AccountId,
            };
            _logger.LogInformation("Sender role retrieved: RoleId={RoleId}", response.RoleId);
            return response;
        }

        public override async Task<UserInfoResponse> GetBasicUserInfo(UserIdRequest request, ServerCallContext context)
        {
            _logger.LogInformation("=== GetBasicUserInfo START === userId: {UserId}, ClientAddress: {ClientAddress}", 
                request.UserId, context.Peer);

            var ressult = await _accountService.GetAccountByIdAsync(request.UserId);
            _logger.LogInformation("Account query result: {IsNull}", ressult == null ? "NULL" : "FOUND");

            var info = new UserInfo(); // default (empty)

            if (ressult == null)
            {
                _logger.LogWarning("Account not found for userId: {UserId}, returning empty UserInfo", request.UserId);
                var emptyResponse = new UserInfoResponse { User = info };
                _logger.LogInformation("=== GetBasicUserInfo END (NULL) === Returning: AccountId={AccountId}, FullName='{FullName}', RoleId={RoleId}", 
                    info.AccountId, info.FullName, info.RoleId);
                return emptyResponse;
            }

            if (ressult.RoleId == 3) // TourGuide
            {
                _logger.LogInformation("Account found - RoleId: {RoleId}, processing as TourGuide", ressult.RoleId);
                var tourGuide = await _tourGuideService.GetTourGuideByAccId(request.UserId);
                if (tourGuide != null)
                {
                    info = new UserInfo
                    {
                        AccountId = tourGuide.AccountId,
                        FullName = tourGuide.FullName,
                        Image = tourGuide.Image ?? "",
                        RoleId = 3 // TourGuide
                    };
                    _logger.LogInformation("TourGuide info set: AccountId={AccountId}, FullName='{FullName}'", 
                        info.AccountId, info.FullName);
                }
                else
                {
                    _logger.LogWarning("TourGuide not found for AccountId: {AccountId}", request.UserId);
                }
            }
            else if (ressult.RoleId == 2) // Customer
            {
                _logger.LogInformation("Account found - RoleId: {RoleId}, processing as Customer", ressult.RoleId);
                var customer = await _customerService.GetCustomerByAccId(request.UserId);
                if (customer != null)
                {
                    info = new UserInfo
                    {
                        AccountId = customer.AccountId,
                        FullName = customer.FullName,
                        Image = customer.Image ?? "",
                        RoleId = 2, // Customer
                    };
                    _logger.LogInformation("Customer info set: AccountId={AccountId}, FullName='{FullName}'", 
                        info.AccountId, info.FullName);
                }
                else
                {
                    _logger.LogWarning("Customer not found for AccountId: {AccountId}", request.UserId);
                }
            }
            else
            {
                _logger.LogWarning("Unknown RoleId: {RoleId} for AccountId: {AccountId}", ressult.RoleId, request.UserId);
            }

            var response = new UserInfoResponse { User = info };
            _logger.LogInformation("=== GetBasicUserInfo END === Returning: AccountId={AccountId}, FullName='{FullName}', RoleId={RoleId}", 
                info.AccountId, info.FullName, info.RoleId);
            return response;
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

        public override async Task<TourGuideResponse> GetTourGuideById(GetTourGuideByIdRequest request, ServerCallContext context)
        {
            try
            {
                _logger.LogInformation("Getting tour guide with ID: {TourGuideId}", request.TourGuideId);

                var tourGuide = await _tourGuideService.GetTourGuideById(request.TourGuideId);

                if (tourGuide == null)
                {
                    _logger.LogWarning("Tour guide with ID {TourGuideId} not found", request.TourGuideId);
                    throw new RpcException(new Status(StatusCode.NotFound, $"Tour guide with ID {request.TourGuideId} not found"));
                }

                var grpcData = new TourGuideResponse
                {
                    TourGuideId = tourGuide.TourGuideId,
                    FullName = tourGuide.FullName,
                    Image = tourGuide.Image,
                    YearOfExperience = tourGuide.YearOfExperience ?? 0,
                    Description = tourGuide.Description,
                    Company = tourGuide.Company,
                    Phone = tourGuide.Phone ?? "",
                };

                _logger.LogInformation("Successfully retrieved tour guide: {UserEmail}", tourGuide.FullName);
                return grpcData;
            }
            catch (RpcException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user with ID: {UserId}", request.TourGuideId);
                throw new RpcException(new Status(StatusCode.Internal, "Internal server error"));
            }
        }

        public override async Task<CustomerResponse> GetCustomerById(GetCustomerByIdRequest request, ServerCallContext context)
        {
            try
            {
                _logger.LogInformation("Getting customer with ID: {CustomerId}", request.CustomerId);

                var customer = await _customerService.GetCustomerById(request.CustomerId);

                if (customer == null)
                {
                    _logger.LogWarning("Customer with ID {CustomerId} not found", request.CustomerId);
                    throw new RpcException(new Status(StatusCode.NotFound, $"Customer with ID {request.CustomerId} not found"));
                }

                var grpcData = new CustomerResponse
                {
                    CustomerId = customer.CustomerId,
                    FullName = customer.FullName,
                    Image = customer.Image,
                    Phone = customer.Phone ?? "",
                    Gender = customer.Gender,
                    Email = customer.Account.Email,
                };

                _logger.LogInformation("Successfully retrieved customer: {UserEmail}", customer.FullName);
                return grpcData;
            }
            catch (RpcException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user with ID: {UserId}", request.CustomerId);
                throw new RpcException(new Status(StatusCode.Internal, "Internal server error"));
            }
        }

        public override async Task<AreaTourGuideResponse> GetNumTourGuideByAreaId(GetAreaDetailRequest request, ServerCallContext context)
        {
            try
            {
                _logger.LogInformation("Getting area detail for AreaId: {AreaId}, Quantity: {Quantity}",
                    request.AreaId, request.TourGuideQuantity);

                var tourGuidesResult = await _tourGuideService.GetTourGuidesByAreaAsync(1, 2, request.AreaId);

                _logger.LogInformation("Retrieved {Count} tour guides from DB before random pick", tourGuidesResult.data.Count);

                var quantity = request.TourGuideQuantity > 0 ? request.TourGuideQuantity : 2;

                var randomTourGuides = tourGuidesResult.data.OrderBy(x => Guid.NewGuid()).Take(quantity).ToList();

                var response = new AreaTourGuideResponse();

                foreach (var guide in randomTourGuides)
                {
                    response.TourGuide.Add(new TourGuideResponse
                    {
                        TourGuideId = guide.TourGuideId,
                        FullName = guide.FullName ?? "",
                        Image = guide.Image ?? "",
                        YearOfExperience = guide.YearOfExperience ?? 0,
                        Description = guide.Description ?? "",
                        Company = guide.Company ?? "",
                        Phone = guide.Phone ?? ""
                    });
                }

                _logger.LogInformation("Response contains {Count} tour guides", response.TourGuide.Count);

                foreach (var guide in response.TourGuide)
                {
                    _logger.LogInformation("GuideId: {Id}, FullName: {Name}, Company: {Company}",
                        guide.TourGuideId, guide.FullName, guide.Company);
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting area detail for AreaId: {AreaId}", request.AreaId);
                throw new RpcException(new Status(StatusCode.Internal, "Internal server error"));
            }
        }
    }
}
