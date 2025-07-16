using System;
using System.Collections.Generic;

namespace TourMate.UserService.Repositories.Models;

public partial class Customer
{
    public int CustomerId { get; set; }

    public string FullName { get; set; } = null!;

    public int AccountId { get; set; }

    public string Gender { get; set; } = null!;

    public DateOnly DateOfBirth { get; set; }

    public string Phone { get; set; } = null!;

    public string? Image { get; set; }

    public virtual Account Account { get; set; } = null!;
}
