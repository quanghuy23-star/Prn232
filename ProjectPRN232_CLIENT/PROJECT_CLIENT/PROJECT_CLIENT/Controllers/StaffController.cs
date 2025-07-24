using Microsoft.AspNetCore.Mvc;
using PROJECT_CLIENT.DTO;
using PROJECT_CLIENT.Service;
using System.Net;

namespace PROJECT_CLIENT.Controllers
{
    public class StaffController : Controller
    {
        private readonly BaseService _baseService = new();

        public async Task<IActionResult> Index()
        {
            var articles = await _baseService.GetData<IEnumerable<ArticleDTO>>("Admin/all");
            return View(articles);
        }

        public async Task<IActionResult> Details(int id)
        {
            var article = await _baseService.GetData<ArticleDTO>($"News/{id}");
            return View(article);
        }

        [HttpPost]
        public async Task<IActionResult> Approve(int id)
        {
            var status = await _baseService.PutData<object>($"Admin/{id}/approve", null);
            if (status == HttpStatusCode.OK)
                TempData["Success"] = "Bài viết đã được duyệt.";
            else
                TempData["Error"] = "Lỗi khi duyệt bài viết.";

            return RedirectToAction("Index", "Staff");
        }

        [HttpPost]
        public async Task<IActionResult> Reject(int id)
        {
            var status = await _baseService.PutData<object>($"Admin/{id}/reject", null);
            if (status == HttpStatusCode.OK)
                TempData["Success"] = "Bài viết đã bị từ chối.";
            else
                TempData["Error"] = "Lỗi khi từ chối bài viết.";

            return RedirectToAction("Index");
        }

      /*  public async Task<IActionResult> Edit(int id)
        {
            var article = await _baseService.GetData<ArticleDTO>($"News/{id}");
            ViewBag.Categories = await _baseService.GetData<IEnumerable<CategoryDTO>>("Category");
            return View(article);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, ArticleDTO article)
        {
            var status = await _baseService.PutData<ArticleDTO>($"Admin/news/{id}", article);
            if (status == HttpStatusCode.OK)
                TempData["Success"] = "Cập nhật thành công.";
            else
                TempData["Error"] = "Cập nhật thất bại.";

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var status = await _baseService.DeleteData($"Admin/news/{id}");
            if (status == HttpStatusCode.OK)
                TempData["Success"] = "Xóa thành công.";
            else
                TempData["Error"] = "Không thể xóa bài viết.";

            return RedirectToAction("Index");
        }*/
    }
}
