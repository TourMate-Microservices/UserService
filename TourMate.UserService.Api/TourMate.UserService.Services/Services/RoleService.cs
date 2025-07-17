using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourMate.UserService.Repositories.IRepositories;
using TourMate.UserService.Repositories.Models;
using TourMate.UserService.Repositories.Repositories;
using TourMate.UserService.Services.IServices;

namespace TourMate.UserService.Services.Services
{
    public class RoleService : IRoleService
    {
        private IRoleRepository _repository;

        public RoleService(IRoleRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<List<Role>> GetAllRoles()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<bool> CreateRole(Role role)
        {
            return await _repository.CreateAsync(role);
        }
    }
}
