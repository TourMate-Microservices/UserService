using Grpc.Core;
using TourMate.UserService.Api.Grpc;
using TourMate.UserService.Services.IServices;

namespace TourMate.UserService.Api.Services
{
    public class UserGrpcService : Grpc.UserService.UserServiceBase
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

        public override async Task<AreaDetailResponse> GetAreaDetail(GetAreaDetailRequest request, ServerCallContext context)
        {
            try
            {
                _logger.LogInformation("Getting area detail for AreaId: {AreaId}, Quantity: {Quantity}", 
                    request.AreaId, request.TourGuideQuantity);

                // Get area info (mock data)
                var areaInfo = GetAreaMockData(request.AreaId);
                if (areaInfo == null)
                {
                    throw new RpcException(new Status(StatusCode.NotFound, $"Area with ID {request.AreaId} not found"));
                }

                // Get tour guides by area
                var tourGuidesResult = await _tourGuideService.GetTourGuidesByAreaAsync(request.AreaId, 1, 20);
                var quantity = request.TourGuideQuantity > 0 ? request.TourGuideQuantity : 2;
                var randomTourGuides = tourGuidesResult.data.OrderBy(x => Guid.NewGuid()).Take(quantity).ToList();

                // Get other areas
                var otherAreas = GetOtherAreasMockData(request.AreaId);

                // Build response
                var response = new AreaDetailResponse
                {
                    AreaId = areaInfo.AreaId,
                    AreaName = areaInfo.AreaName,
                    AreaTitle = areaInfo.AreaTitle,
                    AreaSubtitle = areaInfo.AreaSubtitle,
                    AreaContent = areaInfo.AreaContent,
                    BannerImg = areaInfo.BannerImg
                };

                // Add tour guides
                foreach (var guide in randomTourGuides)
                {
                    response.TourGuide.Add(new TourGuideResponse
                    {
                        TourGuideId = guide.TourGuideId,
                        FullName = guide.FullName,
                        Image = guide.Image,
                        YearOfExperience = guide.YearOfExperience ?? 0,
                        Description = guide.Description,
                        Company = guide.Company ?? ""
                    });
                }

                // Add other areas
                foreach (var area in otherAreas)
                {
                    response.Other.Add(new AreaData
                    {
                        AreaId = area.AreaId,
                        AreaName = area.AreaName,
                        AreaTitle = area.AreaTitle,
                        AreaSubtitle = area.AreaSubtitle,
                        AreaContent = area.AreaContent,
                        BannerImg = area.BannerImg
                    });
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting area detail for AreaId: {AreaId}", request.AreaId);
                throw new RpcException(new Status(StatusCode.Internal, "Internal server error"));
            }
        }

        private AreaMockData? GetAreaMockData(int areaId)
        {
            var areas = GetAllAreasMockData();
            return areas.FirstOrDefault(a => a.AreaId == areaId);
        }

        private List<AreaMockData> GetOtherAreasMockData(int excludeAreaId)
        {
            var areas = GetAllAreasMockData();
            return areas.Where(a => a.AreaId != excludeAreaId).OrderBy(x => Guid.NewGuid()).Take(4).ToList();
        }

        private List<AreaMockData> GetAllAreasMockData()
        {
            return new List<AreaMockData>
            {
                new AreaMockData { AreaId = 1, AreaName = "Hà Nội", AreaTitle = "Thủ đô ngàn năm văn hiến", AreaSubtitle = "Khám phá vẻ đẹp cổ kính", AreaContent = "Hà Nội là thủ đô của Việt Nam với lịch sử hơn 1000 năm", BannerImg = "https://example.com/hanoi.jpg" },
                new AreaMockData { AreaId = 2, AreaName = "TP. Hồ Chí Minh", AreaTitle = "Thành phố năng động", AreaSubtitle = "Trung tâm kinh tế sôi động", AreaContent = "TP.HCM là thành phố lớn nhất Việt Nam", BannerImg = "https://example.com/hcm.jpg" },
                new AreaMockData { AreaId = 3, AreaName = "Đà Nẵng", AreaTitle = "Thành phố đáng sống", AreaSubtitle = "Bãi biển đẹp nhất", AreaContent = "Đà Nẵng nổi tiếng với bãi biển đẹp", BannerImg = "https://example.com/danang.jpg" },
                new AreaMockData { AreaId = 4, AreaName = "Nha Trang", AreaTitle = "Thiên đường biển đảo", AreaSubtitle = "Bãi biển xanh trong", AreaContent = "Nha Trang là điểm du lịch biển đẹp", BannerImg = "https://example.com/nhatrang.jpg" },
                new AreaMockData { AreaId = 5, AreaName = "Hạ Long", AreaTitle = "Di sản thế giới", AreaSubtitle = "Vịnh Hạ Long kỳ vĩ", AreaContent = "Vịnh Hạ Long - di sản thiên nhiên thế giới", BannerImg = "https://example.com/halong.jpg" }
            };
        }

        private class AreaMockData
        {
            public int AreaId { get; set; }
            public string AreaName { get; set; } = "";
            public string AreaTitle { get; set; } = "";
            public string AreaSubtitle { get; set; } = "";
            public string AreaContent { get; set; } = "";
            public string BannerImg { get; set; } = "";
        }
    }
}
