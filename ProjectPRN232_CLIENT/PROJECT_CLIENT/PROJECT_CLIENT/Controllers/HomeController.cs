using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using PROJECT_CLIENT.DTO;
using PROJECT_CLIENT.Models;
using PROJECT_CLIENT.Service;
using System.Diagnostics;
using System.Net;

namespace PROJECT_CLIENT.Controllers
{
    public class HomeController : Controller
    {
        private readonly BaseService _baseService = new();

        public async Task<IActionResult> Index(string? categoryName, string? searchTitle, string? time, int page = 1, int pageSize = 8)
        {
            var articles = await _baseService.GetData<IEnumerable<ArticleDTO>>("News");

            if (!string.IsNullOrEmpty(categoryName))
                articles = articles.Where(a => a.CategoryName == categoryName);

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
            // Phân trang
            int totalItems = articles.Count();
            var pagedArticles = articles.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            ViewBag.CurrentPage = page;
            ViewBag.Categories = await _baseService.GetData<IEnumerable<CategoryDTO>>("News/active-categories");

            return View(pagedArticles);
        }
        [Route("NewDetails/{id}")]
        public async Task<IActionResult> Details(int id,string? slug)
        {
            var article = await _baseService.GetData<ArticleDTO>($"News/{id}");
            var comments = await _baseService.GetData<List<CommentDTO>>($"Comment/news/{id}");
            if (article == null) return NotFound();

            // Kiểm tra slug đúng (optional)
            var expectedSlug = article.NewsTitle.ToLower().Replace(" ", "-");
            if (slug != expectedSlug)
            {
                // Redirect về đúng URL nếu slug sai
                return RedirectToAction("Details", new { id = id, slug = expectedSlug });
            }

            var model = new CommentViewModel
            {
                NewsArticleId = id,
                Comments = comments ?? new List<CommentDTO>()
            };

            ViewBag.Article = article;
            return View(article);
        }
        [Route("NewsByTag/{tagId}")]
        public async Task<IActionResult> NewsByTag(int tagId)
        {
            var articles = await _baseService.GetData<List<ArticleDTO>>($"News/related-by-tagid/{tagId}");
            ViewBag.TagId = tagId;
            return View(articles);
        }

        


         [HttpPost]
         [ValidateAntiForgeryToken]
         public async Task<IActionResult> AddComment(CommentViewModel dto)
         {
             if (!User.Identity.IsAuthenticated)
                 return RedirectToAction("Index", "Login");

             var accessToken = HttpContext.Session.GetString("JWTToken");
             if (string.IsNullOrEmpty(accessToken))
             {
                 TempData["Error"] = "Token hết hạn. Vui lòng đăng nhập lại.";
                 return RedirectToAction("Index", "Login");
             }

             var response = await _baseService.PostWithToken("Comment", dto, accessToken);

             if (response.IsSuccessStatusCode)
                 return RedirectToAction("Details", new { id = dto.NewsArticleId });

             TempData["Error"] = "Gửi bình luận thất bại!";
             return RedirectToAction("Details", new { id = dto.NewsArticleId });
         }

        [HttpPost]
        public async Task<IActionResult> Like(int id)
        {
            var dto = new NewsLikeCreateDTO { NewsArticleId = id };
            var response = await _baseService.PushData("NewsLike", dto); // dùng PostData nếu đã gán sẵn token trong service

            if (response == HttpStatusCode.OK)
                TempData["Success"] = "Đã thích bài viết.";
            else if (response == HttpStatusCode.BadRequest)
                TempData["Error"] = "Bạn đã thích bài viết này.";
            else
                TempData["Error"] = "Lỗi khi gửi lượt thích.";

            return RedirectToAction("Details", new { id = dto.NewsArticleId });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        /*
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
