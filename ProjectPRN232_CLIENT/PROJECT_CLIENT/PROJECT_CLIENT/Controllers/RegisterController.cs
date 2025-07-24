using Microsoft.AspNetCore.Mvc;
using PROJECT_CLIENT.DTO;
using PROJECT_CLIENT.Service;

namespace PROJECT_CLIENT.Controllers
{
    public class RegisterController : Controller
    {
        private readonly BaseService _baseService;

        public RegisterController()
        {
            _baseService = new BaseService();
        }
        [HttpGet]
        public IActionResult Index() => View();

        [HttpPost]
        public async Task<IActionResult> Index(RegisterDTO dto)
        {
            if (!ModelState.IsValid)
                return View("Index", dto); // 🟢 CHỈ ĐỊNH RÕ VIEW

            var response = await _baseService.RegisterAsync(dto);

            if (response.IsSuccess)
            {
                TempData["Success"] = "Đăng ký thành công! Vui lòng đăng nhập.";
                return RedirectToAction("Index", "Login"); // 🟢 Điều hướng đúng
            }

            ModelState.AddModelError(string.Empty, $"Đăng ký thất bại: {response.Error}");
            return View("Index", dto); // 🟢 CHỈ ĐỊNH RÕ VIEW
        }

    }
}
