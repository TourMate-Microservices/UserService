using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourMate.UserService.Repositories.Context;
using TourMate.UserService.Repositories.Models;
using TourMate.UserService.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace TourMate.UserService.Repositories.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private TourMateUserContext _context;

        public RoleRepository()
        {
            _context ??= new();
        }

        public RoleRepository(TourMateUserContext context)
        {
            _context = context;
        }

        public async Task<List<Role>> GetAllAsync()
        {
            return await _context.Roles.ToListAsync();
        }

        public async Task<bool> CreateAsync(Role role)
        {
            try
            {
                _context.Add(role);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
