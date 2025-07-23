namespace ProjectPRN232.DTO
{
    public class NewsArticleUpdateDTO
    {
        public string NewsTitle { get; set; }
        public string Headline { get; set; }
        public string NewsContent { get; set; }
        public string NewsSource { get; set; }
        public int CategoryId { get; set; }
        public string? ImagePath { get; set; }
        public int NewsStatus { get; set; } // 0: Pending, 1: Approved, 2: Rejected
        public List<int> TagIds { get; set; } = new();
    }
}
