using Letters.Domain.Entities;

namespace Letters.Domain.Interfaces;

public interface IEssayRepository
{
    Task<Essay> CreateAsync(Essay essay);
    Task<Essay?> GetByIdAsync(Guid id);
    Task<List<Essay>> GetByUserIdAsync(Guid userId);
    Task<Essay> UpdateAsync(Essay essay);
    Task<bool> DeleteAsync(Guid id);
}
