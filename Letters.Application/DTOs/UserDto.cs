namespace Letters.Application.DTOs
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? PasswordHash { get; set; }
        public string? Document { get; set; }
        public DateOnly BornDate { get; set; }
        public Guid SchoolId { get; set; }
        public int Grade { get; set; }
        public bool isTeacher { get; set; }
    }

    public class UpdateProfileDto
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Document { get; set; } = string.Empty;
        public DateTime BornDate { get; set; }
        public string SchoolId { get; set; } = string.Empty;
        public int Grade { get; set; }
    }

    public class ChangePasswordDto
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }

    public class UserProfileDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Document { get; set; } = string.Empty;
        public DateTime BornDate { get; set; }
        public string SchoolId { get; set; } = string.Empty;
        public int Grade { get; set; }
        public bool IsTeacher { get; set; }
    }
}
