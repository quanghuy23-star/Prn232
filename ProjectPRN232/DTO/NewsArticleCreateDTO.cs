namespace ProjectPRN232.DTO
{
    public class NewsArticleCreateDTO
    {
        public string NewsTitle { get; set; }
        public string Headline { get; set; }
        public string NewsContent { get; set; }
        public string NewsSource { get; set; }
        public int CategoryId { get; set; }

        // Tùy chọn nếu Staff/Admin tạo, mặc định là Pending nếu Writer
      //  public int NewsStatus { get; set; }
    }
}
