﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using ProjectPRN232.DTO;
using ProjectPRN232.Models;
using ProjectPRN232.Service;
using System.Security.Claims;

namespace ProjectPRN232.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private readonly INewsArticleRepository _repo;
        private readonly IMapper _mapper;
        private readonly NewsDbContext _context;

        public NewsController(INewsArticleRepository repo, IMapper mapper, NewsDbContext context)
        {
            _repo = repo;
            _mapper = mapper;
            _context = context;
        }

        [HttpGet]
        [EnableQuery]
       // [Authorize]
        public ActionResult<IQueryable<NewsArticleDTO>> GetAll()
        {
            var data = _repo.GetAll().Where(x => x.NewsStatus == NewsStatus.Approved);
            return Ok(_mapper.ProjectTo<NewsArticleDTO>(data));
        }

        [HttpGet("{id}")]
        //  [Authorize]
        public async Task<ActionResult<NewsArticleDTO>> Get(int id)
        {
            var article = await _repo.GetByIdAsync(id);
            if (article == null || article.NewsStatus != NewsStatus.Approved) return NotFound();
           
            return Ok(_mapper.Map<NewsArticleDTO>(article));
        }
        [HttpPost("{id}/increase-view")]
        public async Task<IActionResult> IncreaseView(int id)
        {
            var article = await _repo.GetByIdAsync(id);
            if (article == null || article.NewsStatus != NewsStatus.Approved)
                return NotFound();

            article.ViewCount += 1;
            await _repo.UpdateAsync(article);

            return NoContent(); // HTTP 204
        }


        [HttpPost("create")]
        [Authorize(Roles = "Writer")]
        public async Task<ActionResult<NewsArticleDTO>> Create([FromBody] NewsArticleCreateDTO dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var article = _mapper.Map<NewsArticle>(dto);
            article.CreatedDate = DateTime.Now;
            article.NewsStatus = NewsStatus.Pending;
            article.CreatedById = userId;

            // Gắn tags từ danh sách TagIds
            if (dto.TagIds != null && dto.TagIds.Any())
            {
                var tags = await _context.Tags
                    .Where(t => dto.TagIds.Contains(t.TagId))
                    .ToListAsync();

                article.Tags = tags;
            }

            await _repo.AddAsync(article);

            // Truy vấn lại để có đầy đủ dữ liệu navigation
            var created = await _context.NewsArticles
                .Include(n => n.Category)
                .ThenInclude(c => c.ParentCategory)
                .Include(n => n.CreatedBy)
                .Include(n => n.UpdatedBy)
                .Include(n => n.Tags)
                .FirstOrDefaultAsync(n => n.NewsArticleId == article.NewsArticleId);

            var result = _mapper.Map<NewsArticleDTO>(created);

            return CreatedAtAction(nameof(Get), new { id = result.NewsArticleId }, result);
        }
        [HttpPut("update/{id}")]
        [Authorize(Roles = "Writer")]
        public async Task<ActionResult> UpdateByWriter(int id, [FromBody] NewsArticleUpdateDTO dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var article = await _context.NewsArticles
                .Include(n => n.Tags)
                .FirstOrDefaultAsync(n => n.NewsArticleId == id);

            if (article == null)
                return NotFound("Bài viết không tồn tại.");

            if (article.CreatedById != userId)
                return StatusCode(StatusCodes.Status403Forbidden, "Bạn không có quyền sửa bài viết này.");
            // Cập nhật nội dung
            article.NewsTitle = dto.NewsTitle;
            article.Headline = dto.Headline;
            article.NewsContent = dto.NewsContent;
            article.NewsSource = dto.NewsSource;
            article.CategoryId = dto.CategoryId;
            article.ImagePath = dto.ImagePath;
            article.ModifiedDate = DateTime.Now;
            article.UpdatedById = userId;
            article.NewsStatus = NewsStatus.Pending; // reset về chờ duyệt

            // Cập nhật Tag
            if (dto.TagIds != null)
            {
                var tags = await _context.Tags
                    .Where(t => dto.TagIds.Contains(t.TagId))
                    .ToListAsync();

                article.Tags = tags;
            }


            await _repo.UpdateAsync(article);

            return Ok(new { Message = "Cập nhật bài viết thành công. Bài viết sẽ được duyệt lại." });
        }




        [HttpGet("my-articles")]
        [Authorize(Roles = "Writer")]
        public ActionResult<IEnumerable<NewsArticleDTO>> GetMyArticles()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var result = _repo.GetAll().Where(x => x.CreatedById == userId);
            return Ok(_mapper.ProjectTo<NewsArticleDTO>(result));
        }
        [HttpGet("my-articles/{id}")]
        //  [Authorize]
        public async Task<ActionResult<NewsArticleDTO>> GetMyArticle(int id)
        {
            var article = await _repo.GetByIdAsync(id);
            if (article == null) return NotFound();

            return Ok(_mapper.Map<NewsArticleDTO>(article));
        }
        [HttpGet("search")]
        [Authorize]
        public ActionResult<IEnumerable<NewsArticleDTO>> Search(string keyword)
        {
            var result = _repo.GetAll()
                .Where(x => x.NewsStatus == NewsStatus.Approved && x.NewsTitle.Contains(keyword));
            return Ok(_mapper.ProjectTo<NewsArticleDTO>(result));
        }
        [HttpGet("active-categories")]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetActiveCategories()
        {
            var categories = await _context.Categories
                .Where(c => c.IsActive)
                .ToListAsync();

            var result = _mapper.Map<IEnumerable<CategoryDTO>>(categories);
            return Ok(result);
        }
        [HttpGet("tags")]
        public async Task<ActionResult<IEnumerable<TagDTO>>> GetAllTags()
        {
            var tags = await _context.Tags
                .ToListAsync();

            var result = _mapper.Map<IEnumerable<TagDTO>>(tags);
            return Ok(result);
        }

        [HttpGet("related-by-tagid/{tagId}")]
        public async Task<IActionResult> GetArticlesByTagId(int tagId)
        {
            var relatedArticles = await _context.NewsArticles
                .Where(n =>
                    n.NewsStatus == NewsStatus.Approved &&
                    n.Tags.Any(t => t.TagId == tagId))
                .OrderByDescending(n => n.CreatedDate)
                .Select(n => new NewsArticleDTO
                {
                    NewsArticleId = n.NewsArticleId,
                    NewsTitle = n.NewsTitle,
                    Headline = n.Headline,
                    CreatedDate = n.CreatedDate,
                    NewsContent = n.NewsContent,
                    NewsSource = n.NewsSource,
                    ImagePath = n.ImagePath,
                    ViewCount = n.ViewCount ?? 0,
                    NewsStatus = (int)n.NewsStatus,
                    NewsStatusName = n.NewsStatus.ToString(),
                    CategoryId = n.CategoryId,
                    CategoryName = n.Category.CategoryName,
                    ParentCategoryName = n.Category.ParentCategory != null ? n.Category.ParentCategory.CategoryName : null,
                    CreatedByName = n.CreatedBy.AccountName,
                    UpdatedByName = n.UpdatedBy != null ? n.UpdatedBy.AccountName : null,
                    ModifiedDate = n.ModifiedDate,
                    Tags = n.Tags.Select(t => new TagDTO
                    {
                        TagId = t.TagId,
                        TagName = t.TagName,
                        Note = t.Note
                    }).ToList()
                })
                .ToListAsync();

            return Ok(relatedArticles);
        }




    }


}
