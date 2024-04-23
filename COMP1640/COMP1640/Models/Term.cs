using System;
using System.Collections.Generic;

namespace COMP1640.Models;

public partial class Term
{
    public int TermId { get; set; }

    public string TermName { get; set; } = null!;

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<Article> Articles { get; set; } = new List<Article>();
}
