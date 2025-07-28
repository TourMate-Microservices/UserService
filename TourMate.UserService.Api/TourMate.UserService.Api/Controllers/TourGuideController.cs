using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TourMate.UserService.Api.Services;
using TourMate.UserService.Repositories.Models;
using TourMate.UserService.Repositories.ResponseModels;
using TourMate.UserService.Services.IServices;
using TourMate.UserService.Services.Services;

namespace TourMate.UserService.Api.Controllers
{
    [Route("api/v1/tour-guides")]
    [ApiController]
    public class TourGuideController : ControllerBase
    {
        private readonly ITourGuideService _tourGuideService;
        private readonly ITourServiceGrpcClient _tourServiceGrpcClient;


        public TourGuideController(ITourGuideService tourGuideService, ITourServiceGrpcClient tourServiceGrpcClient)
        {
            _tourGuideService = tourGuideService ?? throw new ArgumentNullException(nameof(tourGuideService));
            _tourServiceGrpcClient = tourServiceGrpcClient ?? throw new ArgumentNullException(nameof(tourServiceGrpcClient));
        }

        [HttpGet("get-list")]
        public async Task<ActionResult<PagedResult<TourGuide>>> GetFromClient(int? areaId, string? name = "", int pageSize = 10, int pageIndex = 1)
        {
            var result = await _tourGuideService.GetList(pageSize, pageIndex, name, areaId);
            return Ok(result);
        }

        [HttpGet("get-by-area")]
        public async Task<IActionResult> GetTourGuidesByArea([FromQuery] int pageIndex, [FromQuery] int pageSize, [FromQuery] int areaId)
        {
            try
            {
                var result = await _tourGuideService.GetTourGuidesByAreaAsync(pageIndex, pageSize, areaId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Something went wrong", error = ex.Message });
            }
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetPagedTourGuides([FromQuery] string? fullName, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _tourGuideService.GetPagedTourGuidesAsync(pageIndex, pageSize, fullName);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TourGuide>> getById(int id)
        {
            try
            {
                var result = await _tourGuideService.GetTourGuideById(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Something went wrong", error = ex.Message });
            }
        }

        [HttpGet("profile/{id}")]
        public async Task<ActionResult<TourGuideProfileResponse>> GetTourGuideForProfile(int id)
        {
            try
            {
                var tourGuide = await _tourGuideService.GetTourGuideById(id);
                var result = new TourGuideProfileResponse
                {
                    TourGuideId = tourGuide.TourGuideId,
                    FullName = tourGuide.FullName,
                    Image = tourGuide.Image,
                    BannerImage = tourGuide.BannerImage,
                    Description = tourGuide.Description,
                    YearOfExperience = tourGuide.YearOfExperience,
                    Company = tourGuide.Company,
                    AreaId = tourGuide.AreaId,
                    AccountId = tourGuide.AccountId,
                    Address = tourGuide.Address,
                    Phone = tourGuide.Phone,
                    IsVerified = tourGuide.IsVerified,
                    BankAccountNumber = tourGuide.BankAccountNumber,
                    BankName = tourGuide.BankName,
                    DateOfBirth = tourGuide.DateOfBirth,
                    Gender = tourGuide.Gender,                    
                };
                var tours = await _tourServiceGrpcClient.GetToursByTourGuideIdAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Something went wrong", error = ex.Message });
            }
        }

        [HttpGet("other")]
        public async Task<IActionResult> GetOtherTourGuide([FromQuery] int tourGuideId, [FromQuery] int pageSize)
        {
            try
            {
                var result = await _tourGuideService.GetOtherTourGuidesAsync(tourGuideId, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Something went wrong", error = ex.Message });
            }
        }

        [HttpGet("tourguide-with-tours")]
        public async Task<IActionResult> GetTourGuidesWithTours(
    [FromQuery] int numOfTourGuides,
    [FromQuery] int numOfTours)
        {
            try
            {
                // 1. Lấy tour guide ngẫu nhiên (không exclude ai cả)
                var guides = await _tourGuideService.GetRandomTourGuidesAsync(numOfTourGuides);

                if (guides == null || !guides.Any())
                    return NotFound(new { message = "No tour guides found." });

                var results = new List<TourGuideWithTour>();

                foreach (var guide in guides)
                {
                    // 2. Lấy tour từ gRPC
                    var grpcResponse = await _tourServiceGrpcClient.GetNumOfTourByTourGuideId(guide.TourGuideId, numOfTours);

                    // 3. Gộp dữ liệu trả về
                    var mapped = new TourGuideWithTour
                    {
                        TourGuideId = guide.TourGuideId,
                        Image = guide.Image,
                        BannerImage = guide.BannerImage,
                        FullName = guide.FullName,
                        Description = guide.Description,
                        YearOfExperience = guide.YearOfExperience,
                        Company = guide.Company,
                        Tours = grpcResponse.Items.Select(t => new TourOfTourGuide
                        {
                            ServiceId = t.ServiceId,
                            ServiceName = t.ServiceName,
                            Title = t.Title,
                            Image = t.Image
                        }).ToList()
                    };

                    results.Add(mapped);
                }

                return Ok(results);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal Server Error", error = ex.Message });
            }
        }

        [HttpGet("by-id-with-tours-paged")]
        public async Task<IActionResult> GetTourGuideByIdWithToursPaged(
    [FromQuery] int id,
    [FromQuery] int page = 1,
    [FromQuery] int perPage = 5)
        {
            try
            {
                // 1. Lấy guide theo ID
                var guide = await _tourGuideService.GetTourGuideById(id);

                if (guide == null)
                    return NotFound(new { message = "Tour guide not found." });

                // 2. Gọi gRPC lấy tất cả tour theo guideId
                var grpcResponse = await _tourServiceGrpcClient.GetToursByTourGuideIdAsync(id);

                var totalTours = grpcResponse.Items.Count;
                var totalPages = (int)Math.Ceiling((double)totalTours / perPage);

                // 3. Lấy tour theo trang hiện tại
                var pagedTours = grpcResponse.Items
                    .Skip((page - 1) * perPage)
                    .Take(perPage)
                    .Select(t => new TourOfTourGuide
                    {
                        ServiceId = t.ServiceId,
                        ServiceName = t.ServiceName,
                        Title = t.Title,
                        Image = t.Image,
                        CreatedDate = DateTime.TryParse(t.CreatedDate, out var createdDate) ? createdDate : default,

                    }).ToList();

                // 4. Gộp dữ liệu
                var result = new TourGuideDetailWithTour
                {
                    TourGuide  = guide,

                    Tours = new PagedResult<TourOfTourGuide>
                    {
                        data = pagedTours,
                        total_count = totalTours,
                        page = page,
                        per_page = perPage,
                        total_pages = totalPages,
                        has_next = page < totalPages,
                        has_previous = page > totalPages
                    }
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal Server Error", error = ex.Message });
            }
        }
    }
}
