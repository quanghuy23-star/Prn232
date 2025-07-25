using Microsoft.AspNetCore.Mvc;
using PROJECT_CLIENT.DTO;
using PROJECT_CLIENT.Service;

namespace PROJECT_CLIENT.Controllers
{
    [Route("Writer")]
    public class WriterController : Controller
    {
        private readonly BaseService _baseService;

        public WriterController(BaseService baseService)
        {
            _baseService = baseService;
        }

        // Hiển thị danh sách bài viết của người dùng
        [HttpGet("MyArticles")]
        public async Task<IActionResult> MyArticles()
        {
            try
            {
                var articles = await _baseService.GetData<List<ArticleDTO>>("News/my-articles");
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

        // Form tạo bài viết
        [HttpGet("Create")]
        public async Task<IActionResult> Create()
        {
            var categories = await _baseService.GetData<List<CategoryDTO>>("News/active-categories");
            var tags = await _baseService.GetData<List<TagDTO>>("News/tags");

            ViewBag.Categories = categories;
            ViewBag.AllTags = tags;

            return View(new NewsArticleCreateDTO());
        }


        [HttpPost("Create")]
        public async Task<IActionResult> Create(NewsArticleCreateDTO model)
        {
            if (!ModelState.IsValid)
            {
                // Load lại danh sách nếu lỗi
                ViewBag.Categories = await _baseService.GetData<List<CategoryDTO>>("News/active-categories");
                ViewBag.AllTags = await _baseService.GetData<List<TagDTO>>("News/tags");
                return View(model);
            }

            var response = await _baseService.PushData<NewsArticleCreateDTO>("News/create", model);
            if (response != null && response == System.Net.HttpStatusCode.Created)
            {
                TempData["Success"] = "Tạo bài viết thành công";
                return RedirectToAction("MyArticles");
            }

            TempData["Error"] = "Lỗi khi tạo bài viết.";
            ViewBag.Categories = await _baseService.GetData<List<CategoryDTO>>("News/active-categories");
            ViewBag.AllTags = await _baseService.GetData<List<TagDTO>>("News/tags");
            return View(model);
        }


        // Form sửa bài viết
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var article = await _baseService.GetData<NewsArticleUpdateDTO>($"News/my-articles/{id}");
            if (article == null) return NotFound();

            ViewBag.Categories = await _baseService.GetData<List<CategoryDTO>>("News/active-categories");
            ViewBag.AllTags = await _baseService.GetData<List<TagDTO>>("News/tags");
            return View(article);
        }

        [HttpPost("Edit/{id}")]
        public async Task<IActionResult> Edit(int id, NewsArticleUpdateDTO model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _baseService.GetData<List<CategoryDTO>>("News/active-categories");
                ViewBag.AllTags = await _baseService.GetData<List<TagDTO>>("News/tags");
                return View(model);
            }

            var result = await _baseService.PutData($"News/update/{id}", model);
            if (result == System.Net.HttpStatusCode.OK)
            {
                TempData["Success"] = "Cập nhật bài viết thành công!";
                return RedirectToAction("MyArticles");
            }

            TempData["Error"] = "Cập nhật thất bại!";
            ViewBag.Categories = await _baseService.GetData<List<CategoryDTO>>("News/active-categories");
            ViewBag.AllTags = await _baseService.GetData<List<TagDTO>>("Tag");
            return View(model);
        }


    }

}
