namespace ProjectPRN232.DTO
{
    public class CategoryDTO
    {
        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public string? CategoryDescription { get; set; }

        public int? ParentCategoryId { get; set; }

        public string? ParentCategoryName { get; set; }

        public bool IsActive { get; set; }

        // Danh sách danh mục con (đệ quy)
        public List<CategoryDTO>? Children { get; set; } = new();
    }
}
