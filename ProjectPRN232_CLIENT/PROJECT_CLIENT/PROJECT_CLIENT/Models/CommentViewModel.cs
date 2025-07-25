namespace PROJECT_CLIENT.Models
{
    public class CommentViewModel
    {
        public int NewsArticleId { get; set; }
        public int? ParentCommentId { get; set; }
        public string CommentText { get; set; } = string.Empty;

        public List<CommentDTO> Comments { get; set; } = new();
    }

    public class CommentDTO
    {
        public int CommentId { get; set; }
        public int NewsArticleId { get; set; }
        public string CommentText { get; set; } = string.Empty;
        public string CreatedByName { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public List<CommentDTO> Replies { get; set; } = new();
    }

}
