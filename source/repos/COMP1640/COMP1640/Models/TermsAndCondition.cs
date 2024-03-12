using System;
using System.Collections.Generic;

namespace COMP1640.Models;

public partial class TermsAndCondition
{
    public int TermsAndConditionsId { get; set; }

    public string? Content { get; set; }

    public string? Version { get; set; }

    public DateOnly? ReleaseDate { get; set; }
}
