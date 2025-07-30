using Microsoft.EntityFrameworkCore;
using TourMate.UserService.Repositories.Context;
using TourMate.UserService.Repositories.IRepositories;
using TourMate.UserService.Repositories.Models;
using TourMate.UserService.Repositories.RequestModels;
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

        public async Task<PagedResult<TourGuide>> GetList(int pageSize, int pageIndex, string? name, int? areaId)
        {
            name = name?.ToLower() ?? "";

            var query = _context.TourGuides
                .Where(x =>
                    (string.IsNullOrEmpty(name) || x.FullName.ToLower().Contains(name)) &&
                    (areaId == null || x.AreaId == areaId)
                );

            // 👇 Lấy tổng số kết quả TRƯỚC khi phân trang
            var totalResult = await query.CountAsync();

            var result = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var totalPages = (int)Math.Ceiling((double)totalResult / pageSize);

            return new PagedResult<TourGuide>
            {
                data = result,
                total_count = totalResult,
                page = pageIndex,
                per_page = pageSize,
                total_pages = totalPages,
                has_next = pageIndex < totalPages,
                has_previous = pageIndex > 1
            };
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


        public async Task<PagedResult<TourGuide>> GetPagedTourGuide(int pageSize, int pageIndex, string? fullName)
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

        public async Task<bool> UpdateTourGuide(int id, TourGuideUpdateRequest request)
        {
            try
            {
                var checkDuplicatedInfo = _context.TourGuides.Where(x => 
                    x.BankAccountNumber == request.BankAccountNumber || x.Phone == request.Phone
                );
                if (checkDuplicatedInfo.Any(x => x.TourGuideId != id))
                {
                    return false; // Thông tin đã tồn tại
                }
                var currentInfo = checkDuplicatedInfo.FirstOrDefault(x => x.TourGuideId == id);
                if (currentInfo == null)
                {
                    return false; // Không tìm thấy thông tin hiện tại
                }
                currentInfo.FullName = request.FullName;
                currentInfo.Image = request.Image;
                currentInfo.BannerImage = request.BannerImage;
                currentInfo.Description = request.Description;
                currentInfo.YearOfExperience = request.YearOfExperience;
                currentInfo.Company = request.Company;
                currentInfo.AreaId = request.AreaId;
                currentInfo.Address = request.Address;
                currentInfo.Phone = request.Phone;
                currentInfo.BankAccountNumber = request.BankAccountNumber;
                currentInfo.BankName = request.BankName;
                currentInfo.DateOfBirth = request.DateOfBirth;
                currentInfo.Description = request.Description;
                _context.Update(currentInfo);
                await _context.SaveChangesAsync();
                return true; // Cập nhật thành công
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu cần
                return false;
            }
        }
    }
}
