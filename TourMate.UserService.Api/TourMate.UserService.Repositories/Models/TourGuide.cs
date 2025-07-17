using System;
using System.Collections.Generic;

namespace TourMate.UserService.Repositories.Models;

public partial class TourGuide
{
    public int TourGuideId { get; set; }

    public string FullName { get; set; } = null!;

    public string Gender { get; set; } = null!;

    public DateOnly DateOfBirth { get; set; }

    public int AccountId { get; set; }

    public string Address { get; set; } = null!;

    public string Image { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string BannerImage { get; set; } = null!;

    public bool IsVerified { get; set; }

    public string? BankAccountNumber { get; set; }

    public string? BankName { get; set; }

    public int? YearOfExperience { get; set; }

    public string Description { get; set; } = null!;

    public string? Company { get; set; }

    public int AreaId { get; set; }

    public virtual Account Account { get; set; } = null!;
}
