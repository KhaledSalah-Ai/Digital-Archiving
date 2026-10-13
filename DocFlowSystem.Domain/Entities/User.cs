namespace DocFlowSystem.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty; // سنحفظ كلمة المرور مشفرة بأمان
        public string Role { get; set; } = "Employee"; // الصلاحية الافتراضية (مثلاً: Manager أو Employee)
    }
}