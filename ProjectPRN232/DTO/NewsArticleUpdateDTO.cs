namespace ProjectPRN232.DTO
{
    public class NewsArticleUpdateDTO
    {
        public string NewsTitle { get; set; }
        public string Headline { get; set; }
        public string NewsContent { get; set; }
        public string NewsSource { get; set; }
        public int CategoryId { get; set; }
        public bool NewsStatus { get; set; }  // true: active, false: inactive
    }
}
