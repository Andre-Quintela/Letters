namespace Letters.Application.DTOs
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? PasswordHash { get; set; }
        public string? Document { get; set; }
        public DateTime BornDate { get; set; }
        public Guid SchoolId { get; set; }
        public int Grade { get; set; }
        public bool isTeacher { get; set; }
    }
}
