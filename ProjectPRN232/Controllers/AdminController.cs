using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectPRN232.DTO;
using ProjectPRN232.Models;
using ProjectPRN232.Service;
using System.Security.Claims;

namespace ProjectPRN232.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly INewsArticleRepository _repo;
        private readonly ISystemAccountRepository _accountRepo;
        private readonly IMapper _mapper;

        public AdminController(INewsArticleRepository repo, ISystemAccountRepository accountRepo, IMapper mapper)
        {
            _repo = repo;
            _accountRepo = accountRepo;
            _mapper = mapper;
        }

        [HttpGet("all")]
        [Authorize(Roles = "Staff,Admin")]
        public ActionResult<IEnumerable<NewsArticleDTO>> GetAllArticles()
        {
            var result = _repo.GetAll();
            return Ok(_mapper.ProjectTo<NewsArticleDTO>(result));
        }

        [HttpPut("update/{id}")]
        [Authorize(Roles = "Staff")]
        public async Task<ActionResult> Update(int id, [FromBody] NewsArticleUpdateDTO dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return NotFound();

            _mapper.Map(dto, existing);
            existing.UpdatedById = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            existing.ModifiedDate = DateTime.Now;
            await _repo.UpdateAsync(existing);

            return NoContent();
        }

        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "Staff")]
        public async Task<ActionResult> Delete(int id)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return NotFound();

            await _repo.DeleteAsync(existing);
            return NoContent();
        }

        [HttpPatch("{id}/approve")]
        [Authorize(Roles = "Staff")]
        public async Task<ActionResult> Approve(int id)
        {
            var article = await _repo.GetByIdAsync(id);
            if (article == null) return NotFound();

            article.NewsStatus = NewsStatus.Approved;
            article.UpdatedById = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            article.ModifiedDate = DateTime.Now;

            await _repo.UpdateAsync(article);
            return Ok("Approved.");
        }

        [HttpPatch("{id}/reject")]
        [Authorize(Roles = "Staff")]
        public async Task<ActionResult> Reject(int id)
        {
            var article = await _repo.GetByIdAsync(id);
            if (article == null) return NotFound();

            article.NewsStatus = NewsStatus.Rejected;
            article.UpdatedById = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            article.ModifiedDate = DateTime.Now;

            await _repo.UpdateAsync(article);
            return Ok("Rejected.");
        }
        //set-role
        [HttpPut("set-role")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SetUserRole([FromBody] SetRoleDTO dto)
        {
            var account = await _accountRepo.GetByIdAsync(dto.AccountId);

            if (account == null)
                return NotFound("Tài khoản không tồn tại.");

            if (!Enum.IsDefined(typeof(Role), dto.Role))
                return BadRequest("Vai trò không hợp lệ.");

            var currentAdminId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (account.AccountId == currentAdminId)
                return BadRequest("Không thể thay đổi vai trò của chính bạn.");
            

            account.AccountRole= dto.Role;
            _accountRepo.Update(account);
            await _accountRepo.SaveChangesAsync();

            return Ok(new
            {
                Message = $"Đã cập nhật vai trò cho {account.AccountName} thành {(Role)dto.Role}"
            });
        }




    }

}
