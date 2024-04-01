using System;
using System.Collections.Generic;

namespace COMP1640.Models;

public partial class Semester
{
    public int SemesterId { get; set; }

    public DateTime ClosureDate { get; set; }

    public int? ArticleId { get; set; }

    public string? SemesterYear { get; set; }

    public virtual Article? Article { get; set; }
}
