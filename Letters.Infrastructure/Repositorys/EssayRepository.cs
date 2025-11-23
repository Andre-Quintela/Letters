using Letters.Domain.Entities;
using Letters.Domain.Interfaces;
using Letters.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Letters.Infrastructure.Repositorys;

public class EssayRepository : IEssayRepository
{
    private readonly ApplicationDbContext _context;

    public EssayRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Essay> CreateAsync(Essay essay)
    {
        _context.Essays.Add(essay);
        await _context.SaveChangesAsync();
        return essay;
    }

    public async Task<Essay?> GetByIdAsync(Guid id)
    {
        return await _context.Essays
            .Include(e => e.User)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<List<Essay>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Essays
            .Where(e => e.UserId == userId)
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync();
    }

    public async Task<Essay> UpdateAsync(Essay essay)
    {
        _context.Essays.Update(essay);
        await _context.SaveChangesAsync();
        return essay;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var essay = await GetByIdAsync(id);
        if (essay == null) return false;

        _context.Essays.Remove(essay);
        await _context.SaveChangesAsync();
        return true;
    }
}
