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
        public string PhoneNumber { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string Gender { get; set; }
    }
}
