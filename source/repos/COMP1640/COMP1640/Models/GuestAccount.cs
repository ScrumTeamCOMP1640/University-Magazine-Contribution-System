using System;
using System.Collections.Generic;

namespace COMP1640.Models;

public partial class GuestAccount
{
    public int GuestId { get; set; }

    public int? FacultyId { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public virtual Faculty? Faculty { get; set; }
}
