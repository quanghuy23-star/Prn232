using System.ComponentModel.DataAnnotations;

namespace PROJECT_CLIENT.DTO
{
    public class RegisterDTO
    {
        [Required]
        [StringLength(50)]
        public string AccountName { get; set; }
        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string AccountEmail { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string AccountPassword { get; set; }
      //  public int AccountRole { get; set; }  // -- 1: Staff, 2: Writer, 3: Admin 4:Reader
    }
}
