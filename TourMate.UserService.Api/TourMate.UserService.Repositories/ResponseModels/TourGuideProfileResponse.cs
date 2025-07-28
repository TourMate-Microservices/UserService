using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourMate.UserService.Repositories.ResponseModels
{
    public class TourGuideProfileResponse
    {
        public int TourGuideId { get; set; }
        public string FullName { get; set; }
        public string Image { get; set; }
        public string BannerImage { get; set; }
        public string Description { get; set; }
        public int? YearOfExperience { get; set; }
        public string? Company { get; set; }
        public int AreaId { get; set; }
        public int AccountId { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public bool IsVerified { get; set; }
        public string? BankAccountNumber { get; set; }
        public string? BankName { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string Gender { get; set; }
        public PagedResult<TourOfTourGuide> Tours { get; set; } = new PagedResult<TourOfTourGuide>();
    }
}
