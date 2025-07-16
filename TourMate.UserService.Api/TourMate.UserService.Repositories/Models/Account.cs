using System;
using System.Collections.Generic;

namespace TourMate.UserService.Repositories.Models;

public partial class Account
{
    public int AccountId { get; set; }

    public string Email { get; set; } = null!;

    public string? Password { get; set; }

    public DateTime CreatedDate { get; set; }

    public int RoleId { get; set; }

    public bool Status { get; set; }

    public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<TourGuide> TourGuides { get; set; } = new List<TourGuide>();
}
