using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourMate.UserService.Repositories.ResponseModels
{
    public class TourGuideWithTour
    {
        public int TourGuideId { get; set; }
        public string Image { get; set; } = null!;
        public string BannerImage { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int? YearOfExperience { get; set; } = null!;
        public string Company { get; set; } = null!;

        public List<TourOfTourGuide> Tours { get; set; } = new();
    }


    public class TourOfTourGuide
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Image { get; set; } = null!;
    }
}
