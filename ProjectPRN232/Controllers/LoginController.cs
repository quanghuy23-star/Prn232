using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProjectPRN232.DTO;
using ProjectPRN232.Models;
using ProjectPRN232.Service;

namespace ProjectPRN232.Controllers
{
    [Route("api/login")]
    [ApiController]
    public class LoginController : Controller
    {
       // private readonly NewsDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly IPasswordHasherService _passwordHasher;
        private readonly ISystemAccountRepository _repo;
        private readonly IMapper _mapper;

        public LoginController(ITokenService tokenService, IPasswordHasherService passwordHasher, ISystemAccountRepository repo, IMapper mapper)
        {
            _tokenService = tokenService;
            _passwordHasher = passwordHasher;
            _repo = repo;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDTO dto)
        {
            var account = await _repo.GetByEmailAsync(dto.AccountEmail);
            if (account == null || !_passwordHasher.VerifyPassword( account.AccountPassword, dto.AccountPassword))
            {
                return Unauthorized("Invalid email or password.");
            }

            var token = _tokenService.GenerateToken(account);
            var response = _mapper.Map<LoginResponseDTO>(account);
            response.Token = token;

            return Ok(response);
        }
    }
}
