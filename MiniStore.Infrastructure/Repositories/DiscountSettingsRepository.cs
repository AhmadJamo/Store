using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;

namespace MiniStore.Infrastructure.Repositories;

public class DiscountSettingsRepository : IDiscountSettingsRepository
{
    private readonly AppDbContext _context;

    public DiscountSettingsRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<DiscountSettings?> GetAsync()
    {
        return await _context.DiscountSettings
            .SingleOrDefaultAsync(x => x.SingletonKey == 1);
    }

    public async Task AddAsync(DiscountSettings settings)
    {
        await _context.DiscountSettings.AddAsync(settings);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}