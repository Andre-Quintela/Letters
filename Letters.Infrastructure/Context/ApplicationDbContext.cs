
using Microsoft.EntityFrameworkCore;
using Letters.Domain.Entities;

namespace Letters.Infrastructure.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        //DbSets
        public DbSet<User> Users { get; set; }
        public DbSet<Essay> Essays { get; set; }
    }
}
