using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        
        public TourGuideController(ITourGuideService tourGuideService)
        {
            _tourGuideService = tourGuideService ?? throw new ArgumentNullException(nameof(tourGuideService));
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
    }
}
