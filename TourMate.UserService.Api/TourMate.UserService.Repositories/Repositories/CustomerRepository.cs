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
    public class CustomerRepository : ICustomerRepository
    {
        private TourMateUserContext _context;

        public CustomerRepository()
        {
            _context ??= new();
        }

        public CustomerRepository(TourMateUserContext context)
        {
            _context = context;
        }

        public async Task<Customer> GetByAccId(int accId)
        {
            return await _context.Customers.FirstOrDefaultAsync(x => x.AccountId == accId);
        }

        public async Task<Customer> GetById(int id)
        {
            return await _context.Customers.Include(a => a.Account).FirstOrDefaultAsync(x => x.CustomerId == id);
        }

        public async Task<Customer> GetByPhone(string phone)
        {
            return await _context.Customers.FirstOrDefaultAsync(x => x.Phone == phone);
        }

        public async Task<bool> CreateAsync(Customer customer)
        {
            try
            {
                _context.Add(customer);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<PagedResult<Customer>> GetPagedCustomer(int pageSize, int pageIndex, string fullName)
        {
            // Tạo truy vấn từ DbSet
            var query = _context.Customers.AsQueryable();

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

            return new PagedResult<Customer>
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

        public async Task<Dictionary<int, Customer>> GetCustomersFromIds(HashSet<int> ids)
        {
            var query = _context.Customers.AsQueryable();
            if (ids != null && ids.Count > 0)
            {
                query = query.Where(c => ids.Contains(c.CustomerId));
            }
            return await query.ToDictionaryAsync(c => c.CustomerId, c => c);
        }
    }
}
