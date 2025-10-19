using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Letters.Domain.Entities
{
    public class User
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
}
