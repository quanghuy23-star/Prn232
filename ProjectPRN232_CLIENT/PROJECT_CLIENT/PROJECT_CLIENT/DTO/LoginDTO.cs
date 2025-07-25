using System.ComponentModel.DataAnnotations;

namespace PROJECT_CLIENT.DTO
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string AccountEmail { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        public string AccountPassword { get; set; }
    }
}
