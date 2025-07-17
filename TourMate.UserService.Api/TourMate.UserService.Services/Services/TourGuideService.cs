using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourMate.UserService.Repositories.IRepositories;
using TourMate.UserService.Repositories.Models;
using TourMate.UserService.Repositories.ResponseModels;
using TourMate.UserService.Services.IServices;

namespace TourMate.UserService.Services.Services
{
    public class TourGuideService : ITourGuideService
    {
        private readonly ITourGuideRepository _repository;

        public TourGuideService(ITourGuideRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<TourGuide> GetTourGuideByAccId(int accId)
        {
            return await _repository.GetByAccId(accId);
        }

        public async Task<bool> CreateTourGuide(TourGuide tourguide)
        {
            return await _repository.CreateAsync(tourguide);
        }

        public async Task<PagedResult<TourGuide>> GetTourGuidesByAreaAsync(int pageIndex, int pageSize, int areaId)
        {
            return await _repository.GetTourGuidesByAreaAsync(pageIndex, pageSize, areaId);
        }

        public async Task<PagedResult<TourGuide>> GetPagedTourGuidesAsync(int pageIndex, int pageSize, string fullName)
        {
            return await _repository.GetPagedTourGuide(pageSize, pageIndex, fullName);
        }
    }
}
