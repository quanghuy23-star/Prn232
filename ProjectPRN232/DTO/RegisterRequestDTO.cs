namespace ProjectPRN232.DTO
{
    public class RegisterRequestDTO
    {
        public string AccountName { get; set; }
        public string AccountEmail { get; set; }
        public string AccountPassword { get; set; }
        public int AccountRole { get; set; }  // -- 1: Staff, 2: Lecturer, 3: Admin 4:Reader
    }
}
