using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectPRN232.DTO;
using ProjectPRN232.Models;

namespace ProjectPRN232.Controllers;

[Route("api/staff/tags")]
[ApiController]
[Authorize(Roles = "Staff")]
public class TagController : ControllerBase
{
    private readonly NewsDbContext _context;

    public TagController(NewsDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult GetAllTags()
    {
        var tags = _context.Tags.Select(t => new TagDTO
        {
            TagId = t.TagId,
            TagName = t.TagName,
            Note = t.Note
        }).ToList();

        return Ok(tags);
    }

    [HttpGet("{id}")]
    public IActionResult GetTag(int id)
    {
        var tag = _context.Tags.Find(id);
        if (tag == null) return NotFound();

        return Ok(new TagDTO
        {
            TagId = tag.TagId,
            TagName = tag.TagName,
            Note = tag.Note
        });
    }

    [HttpPost]
    public IActionResult CreateTag(TagDTO dto)
    {
        var tag = new Tag
        {
            TagName = dto.TagName,
            Note = dto.Note
        };

        _context.Tags.Add(tag);
        _context.SaveChanges();

        return CreatedAtAction(nameof(GetTag), new { id = tag.TagId }, dto);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateTag(int id, TagDTO dto)
    {
        var tag = _context.Tags.Find(id);
        if (tag == null) return NotFound();

        tag.TagName = dto.TagName;
        tag.Note = dto.Note;
        _context.SaveChanges();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteTag(int id)
    {
        var tag = _context.Tags
            .Include(t => t.NewsArticles) // load liên kết nhiều-nhiều
            .FirstOrDefault(t => t.TagId == id);

        if (tag == null) return NotFound();

        // 1. Xóa các liên kết trong bảng trung gian NewsTag
        tag.NewsArticles.Clear();

        // 2. Xóa chính tag
        _context.Tags.Remove(tag);
        _context.SaveChanges();

        return NoContent();
    }

}
