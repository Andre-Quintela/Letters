using Letters.Domain.Entities;

namespace Letters.Domain.Interfaces
{
    public interface IAuthRepository
    {
        Task<User?> GetByEmail(string email);
        Task<User> Create(User user);
        Task<bool> EmailExists(string email);
    }
}
