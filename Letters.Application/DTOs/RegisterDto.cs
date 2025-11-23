namespace Letters.Application.DTOs
{
    public class RegisterDto
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Document { get; set; } = string.Empty;
        public DateOnly BornDate { get; set; }
        public Guid SchoolId { get; set; }
        public int Grade { get; set; }
        public bool IsTeacher { get; set; }
    }
}
