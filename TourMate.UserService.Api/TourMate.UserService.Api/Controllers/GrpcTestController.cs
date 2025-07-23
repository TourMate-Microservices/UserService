using Microsoft.AspNetCore.Mvc;
using TourMate.UserService.Api.Services;

namespace TourMate.UserService.Api.Controllers
{
    [Route("api/v1/grpc-test")]
    [ApiController]
    public class GrpcTestController : ControllerBase
    {
        private readonly ITourServiceGrpcClient _tourServiceGrpcClient;
        private readonly ILogger<GrpcTestController> _logger;

        public GrpcTestController(ITourServiceGrpcClient tourServiceGrpcClient, ILogger<GrpcTestController> logger)
        {
            _tourServiceGrpcClient = tourServiceGrpcClient;
            _logger = logger;
        }

        /// <summary>
        /// Test gRPC call to get tours by tour guide ID
        /// </summary>
        /// <param name="tourGuideId">The ID of the tour guide</param>
        /// <returns>List of tours for the specified tour guide</returns>
        [HttpGet("tours/{tourGuideId}")]
        public async Task<IActionResult> GetToursByTourGuideId(int tourGuideId)
        {
            try
            {
                _logger.LogInformation("Testing gRPC call for tourGuideId: {TourGuideId}", tourGuideId);
                
                var tours = await _tourServiceGrpcClient.GetToursByTourGuideIdAsync(tourGuideId);
                
                var result = new
                {
                    TourGuideId = tourGuideId,
                    TotalTours = tours.Items.Count,
                    Tours = tours.Items.Select(tour => new
                    {
                        tour.ServiceId,
                        tour.ServiceName,
                        tour.Price,
                        tour.Duration,
                        tour.Content,
                        tour.Image,
                        tour.CreatedDate,
                        tour.IsDeleted,
                        tour.Title,
                        tour.TourDesc,
                        tour.AreaId
                    }).ToList()
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error testing gRPC call for tourGuideId: {TourGuideId}", tourGuideId);
                return StatusCode(500, new { message = "Error calling TourService via gRPC", error = ex.Message });
            }
        }

        [HttpGet("num-of-tours")]
        public async Task<IActionResult> GetNumOfTourByTourGuideId(int tourGuideId, int numsOfTour)
        {
            try
            {
                _logger.LogInformation("Testing gRPC call for tourGuideId: {TourGuideId}", tourGuideId);

                var tours = await _tourServiceGrpcClient.GetNumOfTourByTourGuideId(tourGuideId, numsOfTour);

                var result = new
                {
                    TourGuideId = tourGuideId,
                    TotalTours = tours.Items.Count,
                    Tours = tours.Items.Select(tour => new
                    {
                        tour.ServiceId,
                        tour.ServiceName,
                        tour.Price,
                        tour.Duration,
                        tour.Content,
                        tour.Image,
                        tour.CreatedDate,
                        tour.IsDeleted,
                        tour.Title,
                        tour.TourDesc,
                        tour.AreaId
                    }).ToList()
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error testing gRPC call for tourGuideId: {TourGuideId}", tourGuideId);
                return StatusCode(500, new { message = "Error calling TourService via gRPC", error = ex.Message });
            }
        }

        /// <summary>
        /// Health check for gRPC connection
        /// </summary>
        [HttpGet("health")]
        public IActionResult HealthCheck()
        {
            return Ok(new
            {
                message = "gRPC client is configured and ready",
                timestamp = DateTime.UtcNow,
                service = "TourService gRPC Client"
            });
        }
    }
}
