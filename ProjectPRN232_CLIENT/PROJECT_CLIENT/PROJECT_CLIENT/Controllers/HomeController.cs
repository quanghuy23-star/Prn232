using Microsoft.AspNetCore.Mvc;
using PROJECT_CLIENT.DTO;
using PROJECT_CLIENT.Models;
using PROJECT_CLIENT.Service;
using System.Diagnostics;

namespace PROJECT_CLIENT.Controllers
{
    public class HomeController : Controller
    {
        private readonly BaseService _baseService = new();

        public async Task<IActionResult> Index( string? searchTitle, string? time)
        {
            var articles = await _baseService.GetData<IEnumerable<ArticleDTO>>("News");

           /* if (!string.IsNullOrEmpty(categoryName))
                articles = articles.Where(a => a.CategoryName == categoryName);*/

            if (!string.IsNullOrEmpty(searchTitle))
                articles = articles.Where(a => a.NewsTitle.Contains(searchTitle, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(time))
            {
                var now = DateTime.UtcNow;
                articles = time switch
                {
                    "24h" => articles.Where(a => (now - a.CreatedDate).TotalHours <= 24),
                    "7d" => articles.Where(a => (now - a.CreatedDate).TotalDays <= 7),
                    "30d" => articles.Where(a => (now - a.CreatedDate).TotalDays <= 30),
                    _ => articles
                };
            }

           // ViewBag.Categories = await _baseService.GetData<IEnumerable<CategoryDTO>>("Category");

            return View(articles);
        }

        public async Task<IActionResult> Details(int id)
        {
            var article = await _baseService.GetData<ArticleDTO>($"News/{id}");
            return View(article);
        }

       /* [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(CommentCreateDto dto)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            var accessToken = HttpContext.Session.GetString("JWTToken");
            if (string.IsNullOrEmpty(accessToken))
            {
                TempData["Error"] = "Token hết hạn. Vui lòng đăng nhập lại.";
                return RedirectToAction("Login", "Account");
            }

            var response = await _baseService.PostWithToken("Comment", dto, accessToken);

            if (response.IsSuccessStatusCode)
                return RedirectToAction("Details", new { id = dto.ArticleId });

            TempData["Error"] = "Gửi bình luận thất bại!";
            return RedirectToAction("Details", new { id = dto.ArticleId });
        }

        [HttpPost]
        public async Task<IActionResult> EditComment(int id, string content)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            var accessToken = HttpContext.Session.GetString("JWTToken");
            if (string.IsNullOrEmpty(accessToken))
            {
                TempData["Error"] = "Token hết hạn. Vui lòng đăng nhập lại.";
                return RedirectToAction("Login", "Account");
            }

            var dto = new CommentUpdateDTO { Content = content };

            var response = await _baseService.PutWithToken($"Comment/{id}", dto, accessToken);

            if (response.IsSuccessStatusCode)
            {
                // Cần lấy lại ArticleId để redirect
                var comment = await _baseService.GetData<CommentDTO>($"Comment/{id}");
                return RedirectToAction("Details", new { id = comment.ArticleId });
            }

            TempData["Error"] = "Cập nhật bình luận thất bại!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteComment(int id)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            var accessToken = HttpContext.Session.GetString("JWTToken");
            if (string.IsNullOrEmpty(accessToken))
            {
                TempData["Error"] = "Token hết hạn. Vui lòng đăng nhập lại.";
                return RedirectToAction("Login", "Account");
            }

            // Lấy bình luận để lấy ArticleId
            var comment = await _baseService.GetData<CommentDTO>($"Comment/{id}");
            if (comment == null)
            {
                TempData["Error"] = "Không tìm thấy bình luận.";
                return RedirectToAction("Index");
            }

            var response = await _baseService.DeleteWithToken($"Comment/{id}", accessToken);

            if (response.IsSuccessStatusCode)
                return RedirectToAction("Details", new { id = comment.ArticleId });

            TempData["Error"] = "Xoá bình luận thất bại!";
            return RedirectToAction("Details", new { id = comment.ArticleId });
        }*/

    }
}
