using System;
using System.Collections.Generic;

namespace COMP1640.Models;

public partial class FaU
{
    public int FauId { get; set; }

    public int FacultyId { get; set; }

    public int UserId { get; set; }

    public virtual Faculty Faculty { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
