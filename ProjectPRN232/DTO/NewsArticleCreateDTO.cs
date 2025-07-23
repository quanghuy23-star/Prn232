namespace ProjectPRN232.DTO
{
    public class NewsArticleCreateDTO
    {
        public string NewsTitle { get; set; }
        public string Headline { get; set; }
        public string NewsContent { get; set; }
        public string NewsSource { get; set; }
        public int CategoryId { get; set; }
        public string? ImagePath { get; set; }
        public List<int> TagIds { get; set; } = new();

    }
}
