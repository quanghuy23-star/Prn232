using System;
using System.Collections.Generic;

namespace ProjectPRN232.Models;

public partial class NewsArticle
{
    public int NewsArticleId { get; set; }

    public string NewsTitle { get; set; } = null!;

    public string? Headline { get; set; }

    public DateTime CreatedDate { get; set; }

    public string? NewsContent { get; set; }

    public string? NewsSource { get; set; }
    public string? ImagePath { get; set; }
    public int? ViewCount { get; set; }

    public int CategoryId { get; set; }

    public NewsStatus NewsStatus { get; set; }

    public int? CreatedById { get; set; }

    public int? UpdatedById { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual SystemAccount? CreatedBy { get; set; }

    public virtual SystemAccount? UpdatedBy { get; set; }

    public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();
    public virtual ICollection<NewsLike> NewsLikes { get; set; } = new List<NewsLike>();
}
