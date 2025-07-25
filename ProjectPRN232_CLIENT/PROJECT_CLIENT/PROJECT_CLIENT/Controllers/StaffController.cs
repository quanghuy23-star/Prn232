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
            try
            {
                var articles = await _baseService.GetData<IEnumerable<ArticleDTO>>("Admin/all");
                return View(articles);
            }
            catch (UnauthorizedAccessException ex)
            {
                if (ex.Message == "Unauthorized: You must log in.")
                {
                    return RedirectToAction("Index", "Login");
                }

                throw;
            }
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

        // ====================== CATEGORY MANAGEMENT ======================

        // GET: /Staff/Category
        public async Task<IActionResult> Category()
        {
            var categories = await _baseService.GetData<IEnumerable<CategoryDTO>>("staff/categories");
            return View(categories); // Views/Staff/Category.cshtml
        }

        // GET: /Staff/CreateCategory
        public async Task<IActionResult> CreateCategory()
        {
            var categories = await _baseService.GetData<IEnumerable<CategoryDTO>>("staff/categories");

            var parentCategories = categories
                .Where(c => c.ParentCategoryId == null) // chỉ lấy danh mục gốc, không lồng
                .ToList();

            ViewBag.ParentCategories = parentCategories;

            return View(); // Views/Staff/CreateCategory.cshtml
        }

        // POST: /Staff/CreateCategory
        [HttpPost]
        public async Task<IActionResult> CreateCategory(CategoryDTO dto)
        {
            var result = await _baseService.PushData("staff/categories", dto);
            if (result == HttpStatusCode.Created)
            {
                TempData["Success"] = "Tạo danh mục thành công.";
                return RedirectToAction("Category");
            }

            // Load lại danh mục cha nếu lỗi
            var categories = await _baseService.GetData<IEnumerable<CategoryDTO>>("staff/categories");
            ViewBag.ParentCategories = categories
                .Where(c => c.ParentCategoryId == null)
                .ToList();

            ModelState.AddModelError("", "Không thể tạo danh mục.");
            return View(dto);
        }

        // GET: /Staff/EditCategory/5
        public async Task<IActionResult> EditCategory(int id)
        {
            var category = await _baseService.GetData<CategoryDTO>($"staff/categories/{id}");
            if (category == null) return NotFound();

            var categories = await _baseService.GetData<IEnumerable<CategoryDTO>>("staff/categories");

            // Tránh chọn chính nó làm cha
            var parentCategories = categories
                .Where(c => c.CategoryId != id)
                .Where(c => c.ParentCategoryId == null)
                .ToList();

            ViewBag.ParentCategories = parentCategories;

            return View(category); // Views/Staff/EditCategory.cshtml
        }

        // POST: /Staff/EditCategory/5
        [HttpPost]
        public async Task<IActionResult> EditCategory(int id, CategoryDTO dto)
        {
            var result = await _baseService.PutData($"staff/categories/{id}", dto);
            if (result == HttpStatusCode.NoContent)
            {
                TempData["Success"] = "Cập nhật danh mục thành công.";
                return RedirectToAction("Category");
            }

            // Load lại danh mục cha nếu lỗi
            var categories = await _baseService.GetData<IEnumerable<CategoryDTO>>("staff/categories");

            var parentCategories = categories
                .Where(c => c.CategoryId != id)
                .Where(c => c.ParentCategoryId == null)
                .ToList();

            ViewBag.ParentCategories = parentCategories;

            ModelState.AddModelError("", "Không thể cập nhật danh mục.");
            return View(dto);
        }


        // GET: /Staff/DeleteCategory/5
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var result = await _baseService.DeleteData($"staff/categories/{id}");
            if (result == HttpStatusCode.NoContent)
            {
                TempData["Success"] = "Xóa danh mục thành công.";
            }
            else
            {
                TempData["Error"] = "Không thể xóa danh mục.";
            }

            return RedirectToAction("Category");
        }
        //// Tags
        // GET: /Staff/Tags
        public async Task<IActionResult> Tags()
        {
            var tags = await _baseService.GetData <IEnumerable<TagDTO> > ("staff/tags");
            return View(tags);
        }

        // GET: /Staff/CreateTag
        public IActionResult CreateTag() => View();

        // POST: /Staff/CreateTag
        [HttpPost]
        public async Task<IActionResult> CreateTag(TagDTO dto)
        {
            var status = await _baseService.PushData("staff/tags", dto);
            if (status == HttpStatusCode.Created)
            {
                TempData["Success"] = "Thêm tag thành công";
                return RedirectToAction("Tags");
            }
            TempData["Error"] = "Lỗi khi thêm tag";
            return View(dto);
        }

        // GET: /Staff/EditTag/5
        public async Task<IActionResult> EditTag(int id)
        {
            var tag = await _baseService.GetData<TagDTO>($"staff/tags/{id}");
            return View(tag);
        }

        // POST: /Staff/EditTag/5
        [HttpPost]
        public async Task<IActionResult> EditTag(int id, TagDTO dto)
        {
            var status = await _baseService.PutData($"staff/tags/{id}", dto);
            if (status == HttpStatusCode.NoContent)
            {
                TempData["Success"] = "Cập nhật tag thành công";
                return RedirectToAction("Tags");
            }
            TempData["Error"] = "Lỗi khi cập nhật";
            return View(dto);
        }

        // POST: /Staff/DeleteTag/5
        [HttpPost]
        public async Task<IActionResult> DeleteTag(int id)
        {
            var status = await _baseService.DeleteData($"staff/tags/{id}");
            if (status == HttpStatusCode.NoContent)
            {
                TempData["Success"] = "Xóa tag thành công";
            }
            else
            {
                TempData["Error"] = "Lỗi khi xóa tag";
            }
            return RedirectToAction("Tags");
        }
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

