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
        public async Task<ActionResult> Create([FromBody] NewsArticleCreateDTO dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var article = _mapper.Map<NewsArticle>(dto);
            // CHECK LOG
            Console.WriteLine("Mapped NewsStatus: " + article.NewsStatus);

           // article.NewsStatus = NewsStatus.Pending; // Dù đã ignore AutoMapper thì vẫn nên đặt thủ công

            article.CreatedDate = DateTime.Now;
            article.NewsStatus = NewsStatus.Pending;
            
            article.CreatedById = userId;

            await _repo.AddAsync(article);
            return CreatedAtAction(nameof(Get), new { id = article.NewsArticleId }, article);
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
