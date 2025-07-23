using Microsoft.AspNetCore.Mvc;
using TourMate.UserService.Services.IServices;

namespace TourMate.UserService.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UserGrpcTestController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly ILogger<UserGrpcTestController> _logger;

        public UserGrpcTestController(IAccountService accountService, ILogger<UserGrpcTestController> logger)
        {
            _accountService = accountService;
            _logger = logger;
        }

        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok(new { 
                status = "OK", 
                message = "User gRPC server is running",
                timestamp = DateTime.UtcNow,
                grpcPort = 9092
            });
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUser(int userId)
        {
            try
            {
                _logger.LogInformation("Testing getting user with ID: {UserId}", userId);

                var account = await _accountService.GetAccountByIdAsync(userId);
                
                if (account == null)
                {
                    return NotFound(new { message = $"User with ID {userId} not found" });
                }

                var userInfo = new
                {
                    id = account.AccountId,
                    fullname = account.Email, // Using email as fullname for now
                    email = account.Email,
                    roleId = account.RoleId,
                    status = account.Status,
                    createdDate = account.CreatedDate
                };

                return Ok(userInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user with ID: {UserId}", userId);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
    }
}
