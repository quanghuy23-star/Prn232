using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using ProjectPRN232.DTO;
using ProjectPRN232.Models;
using ProjectPRN232.Service;

namespace ProjectPRN232.Controllers
{
    [Route("api/admin/register")]
    [ApiController]
    public class RegisterController : Controller
    {
        private readonly ISystemAccountRepository _repo;
        private readonly IPasswordHasherService _passwordHasher;

        public RegisterController(ISystemAccountRepository repo, IPasswordHasherService passwordHasher)
        {
            _repo = repo;
            _passwordHasher = passwordHasher;
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO request)
        {
            // Kiểm tra trùng email
            var existingAccount = await _repo.GetByEmailAsync(request.AccountEmail);
            if (existingAccount != null)
            {
                return BadRequest("Email already exists.");
            }

            // Mã hoá mật khẩu
            var hashedPassword = _passwordHasher.HashPassword(request.AccountPassword);


            // Tạo account mới
            var newAccount = new SystemAccount
            {
                AccountName = request.AccountName,
                AccountEmail = request.AccountEmail,
                AccountPassword = hashedPassword,
                AccountRole = request.AccountRole
            };

            await _repo.AddAccountAsync(newAccount);

            return Ok("Account created successfully.");
        }
    }
}
