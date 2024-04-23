using System;
using System.Collections.Generic;

namespace COMP1640.Models;

public partial class Comment
{
    public int CommentId { get; set; }

    public string? CommentContent { get; set; }

    public DateTime? CommentDate { get; set; }

    public int? ArticleId { get; set; }

    public int? UserId { get; set; }

    public virtual Article? Article { get; set; }

    public virtual User? User { get; set; }
}
