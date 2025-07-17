using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourMate.UserService.Repositories.Models;

namespace TourMate.UserService.Services.IServices
{
    public interface IRoleService
    {
        Task<List<Role>> GetAllRoles();
        Task<bool> CreateRole(Role role);
    }
}
