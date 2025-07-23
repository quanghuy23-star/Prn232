namespace ProjectPRN232.Models
{
    public class NewsLike
    {
        public int LikeId { get; set; }

        public int NewsArticleId { get; set; }

        public int LikedById { get; set; }

        public DateTime LikedDate { get; set; }

        public virtual NewsArticle NewsArticle { get; set; } = null!;

        public virtual SystemAccount LikedBy { get; set; } = null!;
    }
}
