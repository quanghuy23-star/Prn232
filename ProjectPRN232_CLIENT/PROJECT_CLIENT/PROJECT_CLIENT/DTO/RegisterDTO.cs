namespace PROJECT_CLIENT.DTO
{
    public class RegisterDTO
    {
        public string AccountName { get; set; }
        public string AccountEmail { get; set; }
        public string AccountPassword { get; set; }
        public int AccountRole { get; set; }  // -- 1: Staff, 2: Writer, 3: Admin 4:Reader
    }
}
