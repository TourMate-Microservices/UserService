using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourMate.UserService.Repositories.Models;

namespace TourMate.UserService.Repositories.IRepositories
{
    public interface IRoleRepository
    {
        Task<bool> CreateAsync(Role role);
        Task<List<Role>> GetAllAsync();
    }
}
