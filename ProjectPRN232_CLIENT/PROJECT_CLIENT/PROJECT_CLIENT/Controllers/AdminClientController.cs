using Microsoft.AspNetCore.Mvc;
using PROJECT_CLIENT.Service;
using PROJECT_CLIENT.DTO;

namespace PROJECT_CLIENT.Controllers
{
    public class AdminClientController : Controller
    {
        private readonly BaseService _baseService;

        public AdminClientController(BaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<IActionResult> AccountList()
        {
            try
            {
                var accounts = await _baseService.GetData<List<SystemAccountDTO>>("Admin/all_Account");
                return View(accounts);
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

        [HttpPost]
        public async Task<IActionResult> SetRole(SetRoleDTO dto)
        {
            var response = await _baseService.PutData<object>("Admin/set-role", dto);
            if (response != null)
                TempData["Success"] = "Đã cập nhật vai trò thành công.";
            else
                TempData["Error"] = "Lỗi khi cập nhật vai trò.";

            return RedirectToAction("AccountList");
        }
    }

}
