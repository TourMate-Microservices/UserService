using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourMate.UserService.Repositories.RequestModels
{
    public class CustomerUpdateRequest
    {
        public string FullName { get; set; }
        public string Phone { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Image { get; set; }
    }
}
