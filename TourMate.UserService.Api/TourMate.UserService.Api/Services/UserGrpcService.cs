using Grpc.Core;
using TourMate.UserService.Services.IServices;
using userservice;

namespace TourMate.UserService.Api.Services
{
    public class UserGrpcService : userservice.UserService.UserServiceBase
    {
        private readonly IAccountService _accountService;
        private readonly ITourGuideService _tourGuideService;
        private readonly ILogger<UserGrpcService> _logger;

        public UserGrpcService(IAccountService accountService, ITourGuideService tourGuideService, ILogger<UserGrpcService> logger)
        {
            _accountService = accountService;
            _tourGuideService = tourGuideService;
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
                        Company = guide.Company ?? ""
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
