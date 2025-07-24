using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using ProjectPRN232.DTO;
using ProjectPRN232.Models;

namespace ProjectPRN232.Controllers;

[Route("api/staff/categories")]
[ApiController]
[Authorize(Roles = "Staff")]
public class CategoryController : ControllerBase
{
    private readonly NewsDbContext _context;
    private readonly IMapper _mapper;

    public CategoryController(NewsDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    // GET: api/staff/categories
    [HttpGet]
    public IActionResult GetAll()
    {
        var categories = _context.Categories
            .Include(c => c.ParentCategory)
            .Include(c => c.InverseParentCategory)
            .ToList();

        var categoryDTOs = _mapper.Map<List<CategoryDTO>>(categories);

        return Ok(categoryDTOs);
    }

    // GET: api/staff/categories/{id}
    [HttpGet("{id}")]
    public IActionResult Get(int id)
    {
        var category = _context.Categories
            .Include(c => c.ParentCategory)
            .Include(c => c.InverseParentCategory)
            .FirstOrDefault(c => c.CategoryId == id);

        if (category == null) return NotFound();

        var dto = _mapper.Map<CategoryDTO>(category);
        return Ok(dto);
    }

    // POST: api/staff/categories
    [HttpPost]
    public IActionResult Create(CategoryDTO dto)
    {
        var category = _mapper.Map<Category>(dto);

        _context.Categories.Add(category);
        _context.SaveChanges();

        var createdDto = _mapper.Map<CategoryDTO>(category);
        return CreatedAtAction(nameof(Get), new { id = category.CategoryId }, createdDto);
    }

    // PUT: api/staff/categories/{id}
    [HttpPut("{id}")]
    public IActionResult Update(int id, CategoryDTO dto)
    {
        var category = _context.Categories.Find(id);
        if (category == null) return NotFound();

        // Update thủ công hoặc dùng AutoMapper
        category.CategoryName = dto.CategoryName;
        category.CategoryDescription = dto.CategoryDescription;
        category.ParentCategoryId = dto.ParentCategoryId;
        category.IsActive = dto.IsActive;

        _context.SaveChanges();
        return NoContent();
    }

    // DELETE: api/staff/categories/{id}
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var category = _context.Categories
            .Include(c => c.NewsArticles)
            .FirstOrDefault(c => c.CategoryId == id);

        if (category == null) return NotFound();

        // 1. Gán ParentCategoryId = null cho các danh mục con
        var childCategories = _context.Categories
            .Where(c => c.ParentCategoryId == id)
            .ToList();

        foreach (var child in childCategories)
        {
            child.ParentCategoryId = null;
        }

        // 2. Gán CategoryId = null cho các bài viết
        foreach (var article in category.NewsArticles)
        {
            article.CategoryId = null;
        }

        // 3. Xóa category
        _context.Categories.Remove(category);
        _context.SaveChanges();

        return NoContent();
    }

}
