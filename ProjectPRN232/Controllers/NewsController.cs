using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
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

        public NewsController(INewsArticleRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        [HttpGet]
        [EnableQuery]
        // [Authorize(Roles ="Staff,Admin")]
       // [Authorize]
        public ActionResult<IQueryable<NewsArticleDTO>> GetAll()
        {
            var data = _repo.GetAll();
            return Ok(_mapper.ProjectTo<NewsArticleDTO>(data));
        }

        [HttpGet("{id}")]
       // [Authorize]
        public async Task<ActionResult<NewsArticleDTO>> Get(int id)
        {
            var article = await _repo.GetByIdAsync(id);
            if (article == null || article.NewsStatus == false) return NotFound();

            return Ok(_mapper.Map<NewsArticleDTO>(article));
        }

        [HttpPost("create")]
        [Authorize(Roles = "Staff,Admin,Lecturer")]
        public async Task<ActionResult> Create([FromBody] NewsArticleCreateDTO dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var article = _mapper.Map<NewsArticle>(dto);
            article.CreatedDate = DateTime.Now;
            article.NewsStatus = false;
            article.CreatedById = userId;

            await _repo.AddAsync(article);
            return CreatedAtAction(nameof(Get), new { id = article.NewsArticleId }, article);
        }

        [HttpPut("update/{id}")]
        [Authorize(Roles = "Staff,Admin,Lecturer")]
        public async Task<ActionResult> Update(int id, [FromBody] NewsArticleUpdateDTO dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return NotFound();

            _mapper.Map(dto, existing);
            existing.ModifiedDate = DateTime.Now;
            existing.UpdatedById = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            await _repo.UpdateAsync(existing);

            return NoContent();
        }

        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(int id)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return NotFound();

            await _repo.DeleteAsync(existing);
            return NoContent();
        }
        //them api 
        //Lấy bài viết theo Category ID
        [HttpGet("by-category/{categoryId}")]
        [Authorize]
        public ActionResult<IEnumerable<NewsArticleDTO>> GetByCategory(int categoryId)
        {
            var result = _repo.GetAll().Where(x => x.CategoryId == categoryId);
            return Ok(_mapper.ProjectTo<NewsArticleDTO>(result));
        }
        //Lấy tất cả bài viết đang hoạt động (NewsStatus = true)
        [HttpGet("active")]
        [Authorize]
        public ActionResult<IEnumerable<NewsArticleDTO>> GetActiveArticles()
        {
            var result = _repo.GetAll().Where(x => x.NewsStatus == true);
            return Ok(_mapper.ProjectTo<NewsArticleDTO>(result));
        }
        //Toggle trạng thái bài viết (active/inactive)
        [HttpPatch("{id}/toggle-status")]
        [Authorize(Roles = "Staff,Admin,Lecturer")]
        public async Task<ActionResult> ToggleStatus(int id)
        {
            var article = await _repo.GetByIdAsync(id);
            if (article == null) return NotFound();

            article.NewsStatus = !article.NewsStatus;
            article.UpdatedById = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            article.ModifiedDate = DateTime.Now;

            await _repo.UpdateAsync(article);
            return NoContent();
        }
        // Lấy top N bài viết mới nhất
        [HttpGet("top-new/{count}")]
        [Authorize]
        public ActionResult<IEnumerable<NewsArticleDTO>> GetTopNewArticles(int count)
        {
            var result = _repo.GetAll()
                .OrderByDescending(x => x.CreatedDate)
                .Take(count);

            return Ok(_mapper.ProjectTo<NewsArticleDTO>(result));
        }
        //Lấy bài viết do người dùng tạo (dựa trên JWT)
        [HttpGet("my-articles")]
        [Authorize]
        public ActionResult<IEnumerable<NewsArticleDTO>> GetMyArticles()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var result = _repo.GetAll().Where(x => x.CreatedById == userId);

            return Ok(_mapper.ProjectTo<NewsArticleDTO>(result));
        }
        //Tìm bài viết theo tiêu đề
        [HttpGet("search")]
        [Authorize]
        public ActionResult<IEnumerable<NewsArticleDTO>> Search(string keyword)
        {
            var result = _repo.GetAll()
                .Where(x => x.NewsTitle.Contains(keyword));

            return Ok(_mapper.ProjectTo<NewsArticleDTO>(result));
        }

    }

}
