using System;
using System.Collections.Generic;

namespace COMP1640.Models;

public partial class Article
{
    public int ArticleId { get; set; }

    public string Title { get; set; } = null!;

    public string? Content { get; set; }

    public string? ImagePath { get; set; }

    public DateTime? SubmissionDate { get; set; }

    public int? UserId { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<SelectedArticle> SelectedArticles { get; set; } = new List<SelectedArticle>();

    public virtual ICollection<Semester> Semesters { get; set; } = new List<Semester>();

    public virtual User? User { get; set; }
}
