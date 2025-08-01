﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourMate.UserService.Repositories.Models;
using TourMate.UserService.Repositories.RequestModels;
using TourMate.UserService.Repositories.ResponseModels;

namespace TourMate.UserService.Services.IServices
{
    public interface ITourGuideService
    {
        Task<PagedResult<TourGuide>> GetList(int pageSize, int pageIndex, string? name, int? areaId);
        Task<List<TourGuide>> GetRandomTourGuidesAsync(int number);
        Task<TourGuide> GetTourGuideByAccId(int accId);
        Task<TourGuide> GetTourGuideById(int id);
        Task<bool> CreateTourGuide(TourGuide tourguide);
        Task<PagedResult<TourGuide>> GetPagedTourGuidesAsync(int pageIndex, int pageSize, string? fullName);
        Task<PagedResult<TourGuide>> GetTourGuidesByAreaAsync(int areaId, int pageIndex, int pageSize);
        Task<List<TourGuide>> GetOtherTourGuidesAsync(int tourGuideId, int pageSize);
        Task<bool> UpdateTourGuide(int id, TourGuideUpdateRequest request);
    }
}
