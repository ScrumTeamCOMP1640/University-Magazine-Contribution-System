using System;
using System.Collections.Generic;

namespace COMP1640.Models;

public partial class SelectedArticle
{
    public int SelectedArticleId { get; set; }

    public int? ArticleId { get; set; }

    public DateOnly? PublicationDate { get; set; }

    public bool? IsPublished { get; set; }

    public virtual Article? Article { get; set; }
}
