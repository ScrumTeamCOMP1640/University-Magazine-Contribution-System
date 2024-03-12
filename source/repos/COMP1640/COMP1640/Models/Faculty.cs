using System;
using System.Collections.Generic;

namespace COMP1640.Models;

public partial class Faculty
{
    public int FacultyId { get; set; }

    public string FacultyName { get; set; } = null!;

    public virtual ICollection<Article> Articles { get; set; } = new List<Article>();

    public virtual ICollection<ClosureDate> ClosureDates { get; set; } = new List<ClosureDate>();

    public virtual ICollection<GuestAccount> GuestAccounts { get; set; } = new List<GuestAccount>();
}
