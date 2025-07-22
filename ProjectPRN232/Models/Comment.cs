using System;
using System.Collections.Generic;

namespace ProjectPRN232.Models;

public partial class Comment
{
    public int CommentId { get; set; }

    public int NewsArticleId { get; set; }

    public int CreatedById { get; set; }

    public string CommentText { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public bool IsActive { get; set; }

    public virtual SystemAccount CreatedBy { get; set; } = null!;

    public virtual NewsArticle NewsArticle { get; set; } = null!;
}
