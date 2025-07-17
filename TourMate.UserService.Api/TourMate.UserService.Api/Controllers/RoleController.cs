using Microsoft.AspNetCore.Mvc;
using TourMate.UserService.Services.IServices;
using TourMate.UserService.Repositories.RequestModels;

namespace TourMate.UserService.Api.Controllers
{
    [Route("api/roles")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var roles = await _roleService.GetAllRoles();
            if (roles == null || !roles.Any())
            {
                return NotFound(new { msg = "Không tìm thấy Role nào." });
            }
            return Ok(roles);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RoleCreate data)
        {
            var role = data.Convert();
            var result = await _roleService.CreateRole(role);
            if(result == false)
            {
                return BadRequest(new { msg = "Tạo Role thất bại!" });
            }
            return Ok(new { msg = "Tạo Role thành công." });
            ;
        }
    }
}
