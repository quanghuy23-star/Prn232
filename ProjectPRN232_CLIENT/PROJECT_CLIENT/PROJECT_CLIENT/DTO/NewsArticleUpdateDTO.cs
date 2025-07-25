namespace PROJECT_CLIENT.DTO
{
    public class NewsArticleUpdateDTO
    {
        public int NewsArticleId { get; set; }
        public string NewsTitle { get; set; }
        public string Headline { get; set; }
        public string NewsContent { get; set; }
        public string NewsSource { get; set; }
        public int? CategoryId { get; set; }
        public string? ImagePath { get; set; }
        public int NewsStatus { get; set; } // 0: Pending, 1: Approved, 2: Rejected
         public List<int> TagIds { get; set; } = new();
        // Thay đổi: nhận danh sách tag object từ API
       // public List<TagDTO> Tags { get; set; } = new();

        // Thêm property get-only để dùng cho View
       // public List<int> TagIds => Tags.Select(t => t.TagId).ToList();
    }
}
