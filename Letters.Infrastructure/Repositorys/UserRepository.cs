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

        public async Task Add(User user)
        {
            await _context.AddAsync(user);
            await _context.SaveChangesAsync();

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

        public async Task<User> getById(Guid id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id) ?? throw new Exception("Usuário não encontrado");
        }

        public async Task Update(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
