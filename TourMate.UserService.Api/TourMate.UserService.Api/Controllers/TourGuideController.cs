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
        public async Task<IActionResult> GetPagedTourGuides([FromQuery] string fullName, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _tourGuideService.GetPagedTourGuidesAsync(pageIndex, pageSize, fullName);
            return Ok(result);
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
    }
}
