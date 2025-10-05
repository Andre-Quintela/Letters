using Letters.Domain.Entities;
using Letters.Domain.Interfaces;
using Letters.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Letters.Infrastructure.Repositorys
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task Add(User user)
        {
            throw new NotImplementedException();
        }

        public Task Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<List<User>> GetAll()
        {
            return _context.Users.ToListAsync();
        }

        public Task<User> getByEmail()
        {
            throw new NotImplementedException();
        }

        public Task<User> getById(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task Update(User user)
        {
            throw new NotImplementedException();
        }
    }
}
