using System;
using System.Collections.Generic;

namespace COMP1640.Models;

public partial class Faculty
{
    public int FacultyId { get; set; }

    public string FacultyName { get; set; } = null!;

    public virtual ICollection<FaU> FaUs { get; set; } = new List<FaU>();
}
