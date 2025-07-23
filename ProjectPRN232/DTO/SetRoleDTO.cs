namespace ProjectPRN232.DTO
{
    public class SetRoleDTO
    {
        public int AccountId { get; set; }
        public int Role { get; set; } // 1: Staff, 2: Writer, 3: Admin, 4: Reader
    }
}
