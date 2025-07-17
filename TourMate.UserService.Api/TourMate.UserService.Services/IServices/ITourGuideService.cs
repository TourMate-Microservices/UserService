using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourMate.UserService.Repositories.Models;
using TourMate.UserService.Repositories.ResponseModels;

namespace TourMate.UserService.Services.IServices
{
    public interface ITourGuideService
    {
        Task<TourGuide> GetTourGuideByAccId(int accId);
        Task<bool> CreateTourGuide(TourGuide tourguide);
        Task<PagedResult<TourGuide>> GetPagedTourGuidesAsync(int pageIndex, int pageSize, string fullName);
        Task<PagedResult<TourGuide>> GetTourGuidesByAreaAsync(int areaId, int pageIndex, int pageSize);
    }
}
