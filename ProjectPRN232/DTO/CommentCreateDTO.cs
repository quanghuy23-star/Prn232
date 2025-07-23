namespace ProjectPRN232.DTO
{
    public class CommentCreateDTO
    {
        public int NewsArticleId { get; set; }
        public string CommentText { get; set; } = null!;
        public int? ParentCommentId { get; set; }
    }
}
