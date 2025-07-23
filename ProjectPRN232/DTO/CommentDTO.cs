namespace ProjectPRN232.DTO
{
    public class CommentDTO
    {
        public int CommentId { get; set; }
        public int NewsArticleId { get; set; }
        public string CommentText { get; set; } = null!;
        public string CreatedByName { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
        public List<CommentDTO> Replies { get; set; } = new();
    }
}
