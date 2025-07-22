using Microsoft.AspNetCore.Mvc;

namespace TourMate.UserService.Api.Controllers
{
    [Route("api/v1/test")]
    [ApiController]
    public class TestController : ControllerBase
    {
        /// <summary>
        /// Simple test endpoint to verify API is working
        /// </summary>
        [HttpGet("health")]
        public IActionResult HealthCheck()
        {
            return Ok(new
            {
                message = "API is working",
                timestamp = DateTime.UtcNow,
                service = "UserService"
            });
        }

        /// <summary>
        /// Test endpoint with parameter
        /// </summary>
        [HttpGet("echo/{message}")]
        public IActionResult Echo(string message)
        {
            return Ok(new
            {
                originalMessage = message,
                upperCase = message.ToUpper(),
                timestamp = DateTime.UtcNow
            });
        }
    }
}
