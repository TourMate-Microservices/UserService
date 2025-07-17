using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourMate.UserService.Repositories.Context;
using TourMate.UserService.Repositories.Models;

namespace TourMate.UserService.Repositories.Repositories
{
    public class TourGuideRepository
    {
        private TourMateUserContext _context;

        public TourGuideRepository()
        {
            _context ??= new();
        }

        public TourGuideRepository(TourMateUserContext context)
        {
            _context = context;
        }

        public async Task<TourGuide> GetByAccId(int accId)
        {
            return await _context.TourGuides.Include(a => a.Account).FirstOrDefaultAsync(x => x.AccountId == accId);
        }

        public async Task<bool> CreateAsync(TourGuide tourGuide)
        {
            try
            {
                _context.Add(tourGuide);
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
