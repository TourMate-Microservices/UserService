using System;
using System.Collections.Generic;

namespace TourMate.UserService.Repositories.Models;

public partial class TourGuideDesc
{
    public int TourGuideDescId { get; set; }

    public int TourGuideId { get; set; }

    public int? YearOfExperience { get; set; }

    public string Description { get; set; } = null!;

    public int AreaId { get; set; }

    public string? Company { get; set; }
}
