using ProjectPRN232.DTO;
using ProjectPRN232.Models;

namespace ProjectPRN232.Mappers
{
    public static class CommentMapper
    {
        public static CommentDTO MapCommentToDTO(Comment comment)
        {
            return new CommentDTO
            {
                CommentId = comment.CommentId,
                NewsArticleId = comment.NewsArticleId,
                CommentText = comment.CommentText,
                CreatedDate = comment.CreatedDate,
                IsActive = comment.IsActive,
                CreatedByName = comment.CreatedBy.AccountName,
                Replies = comment.Replies?
                    .Where(r => r.IsActive)
                    .Select(MapCommentToDTO)
                    //  .Select(r => MapCommentToDTO(r)) // đệ quy lồng nhiều tầng
                    .ToList() ?? new List<CommentDTO>()
            };
        }
    }
}
