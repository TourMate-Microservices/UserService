using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourMate.UserService.Repositories.ResponseModels
{
    public class PagedResult<T>
    {
        public IList<T> data { get; set; }              // Mảng dữ liệu của trang hiện tại
        public int total_count { get; set; }             // Tổng số bản ghi
        public int page { get; set; }                   // Trang hiện tại
        public int per_page { get; set; }                // Số bản ghi/trang
        public int total_pages { get; set; }             // Tổng số trang
        public bool has_next { get; set; }               // Còn trang sau không
        public bool has_previous { get; set; }           // Còn trang trước không
    }
}
