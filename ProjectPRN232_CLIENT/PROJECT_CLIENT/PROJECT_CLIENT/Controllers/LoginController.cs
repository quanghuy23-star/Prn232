using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using PROJECT_CLIENT.DTO;
using PROJECT_CLIENT.Service;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace PROJECT_CLIENT.Controllers
{
    public class LoginController : Controller
    {
        private readonly BaseService _baseService;

        public LoginController(BaseService baseService)
        {
            _baseService = baseService;
        }


        [HttpGet]
        public IActionResult Index() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginDTO dto)
        {
            var token = await _baseService.LoginAsync(dto);

            if (string.IsNullOrEmpty(token))
            {
                ViewBag.Error = "Invalid login.";
                return View();
            }

            HttpContext.Session.SetString("JWTToken", token);

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            var role = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role || c.Type == "role")?.Value ?? "";
            var userId = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier || c.Type == "nameid")?.Value ?? "";

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, dto.AccountEmail),
                new Claim("AccessToken", token),
                new Claim(ClaimTypes.Role, role),
                new Claim(ClaimTypes.NameIdentifier, userId)
            };

            HttpContext.Session.SetString("UserId", userId);

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return role switch
            {
                "Admin" => RedirectToAction("AccountList", "AdminClient"),
                "Staff" => RedirectToAction("Index", "staff"),
                "Writer" => RedirectToAction("MyArticles", "writer"),
                _ => RedirectToAction("Index", "Home"),
            };
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
