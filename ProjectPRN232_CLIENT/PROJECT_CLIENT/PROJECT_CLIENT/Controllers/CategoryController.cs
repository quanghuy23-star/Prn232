using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PROJECT_CLIENT.DTO;
using PROJECT_CLIENT.Service;
using System.Net;

namespace PROJECT_CLIENT.Controllers
{
    [Authorize(Roles = "Staff")]
    public class CategoryController : Controller
    {
        private readonly BaseService _baseService = new();

        public async Task<IActionResult> Index()
        {
            var categories = await _baseService.GetData<IEnumerable<CategoryDTO>>("Category");
            return View(categories);
        }

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(CategoryDTO dto)
        {
            var result = await _baseService.PushData("Category", dto);
            if (result == HttpStatusCode.Created)
                return RedirectToAction("Index");
            ModelState.AddModelError("", "Không thể tạo category.");
            return View(dto);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var category = await _baseService.GetData<CategoryDTO>($"Category/{id}");
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, CategoryDTO dto)
        {
            var result = await _baseService.PutData($"Category/{id}", dto);
            if (result == HttpStatusCode.NoContent)
                return RedirectToAction("Index");
            ModelState.AddModelError("", "Không thể cập nhật category.");
            return View(dto);
        }

        public async Task<IActionResult> Delete(int id)
        {
            await _baseService.DeleteData($"Category/{id}");
            return RedirectToAction("Index");
        }
    }
}
