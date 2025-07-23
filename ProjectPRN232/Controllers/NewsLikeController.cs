using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectPRN232.DTO;
using ProjectPRN232.Models;
using System.Security.Claims;

namespace ProjectPRN232.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize] // Chặn không cho người chưa đăng nhập like
public class NewsLikeController : ControllerBase
{
    private readonly NewsDbContext _context;

    public NewsLikeController(NewsDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> LikeArticle([FromBody] NewsLikeCreateDTO dto)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        var exists = await _context.NewsLikes
            .AnyAsync(nl => nl.NewsArticleId == dto.NewsArticleId && nl.LikedById == userId);

        if (exists)
            return BadRequest("You have already liked this article.");

        var like = new NewsLike
        {
            NewsArticleId = dto.NewsArticleId,
            LikedById = userId,
            LikedDate = DateTime.Now
        };

        _context.NewsLikes.Add(like);
        await _context.SaveChangesAsync();

        return Ok(new { Message = "Liked successfully" });
    }

    [HttpDelete]
    [Authorize]
    public async Task<IActionResult> UnlikeArticle([FromBody] NewsLikeCreateDTO dto)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        var like = await _context.NewsLikes
            .FirstOrDefaultAsync(nl => nl.NewsArticleId == dto.NewsArticleId && nl.LikedById == userId);

        if (like == null)
            return NotFound("Like not found.");

        _context.NewsLikes.Remove(like);
        await _context.SaveChangesAsync();

        return Ok(new { Message = "Unliked successfully" });
    }

    [HttpGet("count/{newsArticleId}")]
    [AllowAnonymous] // Ai cũng xem được số lượt like
    public async Task<ActionResult<int>> GetLikeCount(int newsArticleId)
    {
        var count = await _context.NewsLikes.CountAsync(nl => nl.NewsArticleId == newsArticleId);
        return Ok(count);
    }

    [HttpGet("liked/{newsArticleId}")]
    public async Task<IActionResult> HasUserLiked(int newsArticleId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        var liked = await _context.NewsLikes
            .AnyAsync(nl => nl.NewsArticleId == newsArticleId && nl.LikedById == userId);

        return Ok(liked);
    }
}
