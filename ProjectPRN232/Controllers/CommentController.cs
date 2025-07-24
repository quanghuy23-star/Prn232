using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectPRN232.DTO;
using ProjectPRN232.Mappers;
using ProjectPRN232.Models;
using System.Security.Claims;

namespace ProjectPRN232.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly NewsDbContext _context;

        public CommentController(NewsDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PostComment(CommentCreateDTO dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var comment = new Comment
            {
                NewsArticleId = dto.NewsArticleId,
                CommentText = dto.CommentText,
                CreatedById = userId,
                CreatedDate = DateTime.Now,
                IsActive = true,
                ParentCommentId = dto.ParentCommentId
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Comment posted successfully." });
        }

        /*[HttpGet("news/{newsId}")]
        public async Task<IActionResult> GetCommentsForNews(int newsId)
        {
            var comments = await _context.Comments
                .Include(c => c.CreatedBy)
                .Include(c => c.Replies)
                    .ThenInclude(r => r.Replies)
                    .ThenInclude(rr => rr.CreatedBy)
                .Where(c => c.NewsArticleId == newsId && c.ParentCommentId == null && c.IsActive)
                .ToListAsync();

            var result = comments.Select(CommentMapper.MapCommentToDTO).ToList();
            return Ok(result);
        }*/
        [HttpGet("news/{newsId}")]
        public async Task<IActionResult> GetCommentsForNews(int newsId)
        {
            var comments = await _context.Comments
                .Include(c => c.CreatedBy)
                .Where(c => c.NewsArticleId == newsId && c.IsActive)
                .ToListAsync();

            // Xoá Replies cũ nếu có
            foreach (var c in comments)
                c.Replies = new List<Comment>();

            // Build cây comment
            var commentDict = comments.ToDictionary(c => c.CommentId);

            foreach (var comment in comments)
            {
                if (comment.ParentCommentId != null && commentDict.TryGetValue(comment.ParentCommentId.Value, out var parent))
                {
                    parent.Replies.Add(comment);
                }
            }

            var topLevel = comments.Where(c => c.ParentCommentId == null).ToList();
            var result = topLevel.Select(CommentMapper.MapCommentToDTO).ToList();
            return Ok(result);
        }



        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null || !comment.IsActive) return NotFound();

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var role = User.FindFirstValue(ClaimTypes.Role);

            // Nếu không phải Staff thì chỉ được xóa comment do chính mình tạo
            if (role != "Staff" && comment.CreatedById != userId)
            {
                return Forbid();
            }

            comment.IsActive = false;
            await _context.SaveChangesAsync();
            return NoContent();
        }

    }
}
