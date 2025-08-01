﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourMate.UserService.Repositories.Models;
using TourMate.UserService.Repositories.RequestModels;
using TourMate.UserService.Repositories.ResponseModels;

namespace TourMate.UserService.Repositories.IRepositories
{
    public interface ITourGuideRepository
    {
        Task<PagedResult<TourGuide>> GetList(int pageSize, int pageIndex, string? name, int? areaId);
        Task<List<TourGuide>> GetRandomTourGuidesAsync(int number);
        Task<TourGuide> GetByAccId(int accId);
        Task<TourGuide> GetById(int id);
        Task<bool> CreateAsync(TourGuide tourGuide);
        Task<PagedResult<TourGuide>> GetTourGuidesByAreaAsync(int pageIndex, int pageSize, int areaId);
        Task<PagedResult<TourGuide>> GetPagedTourGuide(int pageSize, int pageIndex, string? fullName);
        Task<List<TourGuide>> GetOtherTourGuides(int excludeId, int pageSize);
        Task<bool> UpdateTourGuide(int id, TourGuideUpdateRequest request);
    }
}
