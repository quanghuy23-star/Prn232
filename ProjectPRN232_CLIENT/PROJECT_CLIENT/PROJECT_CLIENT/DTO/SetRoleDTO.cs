namespace PROJECT_CLIENT.DTO
{
    public class SetRoleDTO
    {
        public int AccountId { get; set; }
        public int Role { get; set; } // 1: Staff, 2: Writer, 3: Admin, 4: Reader
    }
    public enum Role
    {
        Staff = 1,
        Writer = 2,
        Admin = 3,
        Reader = 4
    }
}
