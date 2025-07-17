using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourMate.UserService.Repositories.Models;

namespace TourMate.UserService.Repositories.IRepositories
{
    public interface ITourGuideRepository
    {
        Task<TourGuide> GetByAccId(int accId);
        Task<bool> CreateAsync(TourGuide tourGuide);
    }
}
