using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourMate.UserService.Repositories.Context;
using TourMate.UserService.Repositories.IRepositories;
using TourMate.UserService.Repositories.Models;
using TourMate.UserService.Repositories.ResponseModels;

namespace TourMate.UserService.Repositories.Repositories
{
    public class TourGuideRepository : ITourGuideRepository
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

        public async Task<List<TourGuide>> GetRandomTourGuidesAsync(int number)
        {
            return await _context.TourGuides
                .OrderBy(g => Guid.NewGuid()) // random
                .Take(number)
                .ToListAsync();
        }


        public async Task<TourGuide> GetByAccId(int accId)
        {
            return await _context.TourGuides.Include(a => a.Account).FirstOrDefaultAsync(x => x.AccountId == accId);
        }

        public async Task<TourGuide> GetById(int id)
        {
            return await _context.TourGuides.FirstOrDefaultAsync(x => x.TourGuideId == id);
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

        public async Task<PagedResult<TourGuide>> GetTourGuidesByAreaAsync(int pageIndex, int pageSize, int areaId)
        {
            var query = _context.TourGuides
                .Where(tg => tg.AreaId == areaId);

            // Tổng số bản ghi phù hợp
            var totalCount = await query.CountAsync();

            // Sắp xếp ngẫu nhiên & phân trang
            var result = await query
                .OrderBy(x => Guid.NewGuid()) // sắp xếp ngẫu nhiên
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            return new PagedResult<TourGuide>
            {
                data = result,
                total_count = totalCount,
                page = pageIndex,
                per_page = pageSize,
                total_pages = totalPages,
                has_next = pageIndex < totalPages,
                has_previous = pageIndex > 1
            };
        }


        public async Task<PagedResult<TourGuide>> GetPagedTourGuide(int pageSize, int pageIndex, string fullName)
        {
            // Tạo truy vấn từ DbSet
            var query = _context.TourGuides.AsQueryable();

            // Lọc theo tên nếu có
            if (!string.IsNullOrEmpty(fullName))
            {
                query = query.Where(t => t.FullName.Contains(fullName));
            }

            // Tổng số bản ghi sau khi lọc
            var totalCount = await query.CountAsync();

            // Lấy danh sách dữ liệu trang hiện tại
            var result = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Tính tổng số trang
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            return new PagedResult<TourGuide>
            {
                data = result,
                total_count = totalCount,
                page = pageIndex,
                per_page = pageSize,
                total_pages = totalPages,
                has_next = pageIndex < totalPages,
                has_previous = pageIndex > 1
            };
        }

        public async Task<List<TourGuide>> GetOtherTourGuides(int excludeId, int pageSize)
        {
            var result = await _context.TourGuides
    .Where(x => x.TourGuideId != excludeId)
    .OrderBy(x => Guid.NewGuid())  // Sắp xếp ngẫu nhiên
    .Take(pageSize)  // Giới hạn số lượng kết quả theo pageSize
    .ToListAsync();


            return result;
        }
    }
}
