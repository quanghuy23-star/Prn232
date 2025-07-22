using AutoMapper;
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
        [Authorize]
        public ActionResult<IQueryable<NewsArticleDTO>> GetAll()
        {
            var data = _repo.GetAll().Where(x => x.NewsStatus == NewsStatus.Approved);
            return Ok(_mapper.ProjectTo<NewsArticleDTO>(data));
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<NewsArticleDTO>> Get(int id)
        {
            var article = await _repo.GetByIdAsync(id);
            if (article == null || article.NewsStatus != NewsStatus.Approved) return NotFound();

            return Ok(_mapper.Map<NewsArticleDTO>(article));
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



        [HttpGet("my-articles")]
        [Authorize(Roles = "Writer")]
        public ActionResult<IEnumerable<NewsArticleDTO>> GetMyArticles()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var result = _repo.GetAll().Where(x => x.CreatedById == userId);
            return Ok(_mapper.ProjectTo<NewsArticleDTO>(result));
        }

        [HttpGet("search")]
        [Authorize]
        public ActionResult<IEnumerable<NewsArticleDTO>> Search(string keyword)
        {
            var result = _repo.GetAll()
                .Where(x => x.NewsStatus == NewsStatus.Approved && x.NewsTitle.Contains(keyword));
            return Ok(_mapper.ProjectTo<NewsArticleDTO>(result));
        }
    }


}
