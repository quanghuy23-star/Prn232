namespace ProjectPRN232.DTO
{
    public class NewsArticleDTO
    {
        public int NewsArticleId { get; set; }
        public string NewsTitle { get; set; }
        public string Headline { get; set; }
        public DateTime CreatedDate { get; set; }
        public string NewsContent { get; set; }
        public string NewsSource { get; set; }
        public string NewsStatus { get; set; }
        public string CategoryName { get; set; }
        public string CreatedByName { get; set; }
        public string? UpdatedByName { get; set; }
        public DateTime? ModifiedDate { get; set; }
        //
        public List<string> TagNames { get; set; }
    }
}
