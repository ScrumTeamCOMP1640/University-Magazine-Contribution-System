using System;
using System.Collections.Generic;

namespace COMP1640.Models;

public partial class ClosureDate
{
    public int ClosureDateId { get; set; }

    public int? AcademicYear { get; set; }

    public DateTime? ClosureDate1 { get; set; }

    public int? FacultyId { get; set; }

    public virtual Faculty? Faculty { get; set; }
}
