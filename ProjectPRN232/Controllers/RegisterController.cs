using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using ProjectPRN232.DTO;
using ProjectPRN232.Models;
using ProjectPRN232.Service;

namespace ProjectPRN232.Controllers
{
    [Route("api/[controller]")]
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


        [HttpPost("create")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateAccountForAdmin([FromBody] RegisterRequestDTO request)
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
              //  AccountRole = request.AccountRole
            };

            await _repo.AddAccountAsync(newAccount);

            return Ok("Account created successfully.");
        }
        [HttpPost]
        [AllowAnonymous] // Nếu muốn mở cho tất cả người dùng (không cần login)
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO request)
        {
            var existingAccount = await _repo.GetByEmailAsync(request.AccountEmail);
            if (existingAccount != null)
            {
                return BadRequest(new { message = "Email already exists." }); // ✅ Trả về JSON chuẩn
            }

            var hashedPassword = _passwordHasher.HashPassword(request.AccountPassword);

            var newAccount = new SystemAccount
            {
                AccountName = request.AccountName,
                AccountEmail = request.AccountEmail,
                AccountPassword = hashedPassword,
                AccountRole = 4 // Reader role mặc định
            };

            await _repo.AddAccountAsync(newAccount);

            return Ok(new { message = "Account created successfully.", userId = newAccount.AccountId });

        }

    }
}
