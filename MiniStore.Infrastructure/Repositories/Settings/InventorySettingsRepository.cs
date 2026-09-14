using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;

namespace MiniStore.Infrastructure.Repositories;

public class InventorySettingsRepository(AppDbContext context)
    : IInventorySettingsRepository
{
    public Task<InventorySettings?> GetAsync() => context.InventorySettings
        .SingleOrDefaultAsync(x => x.SingletonKey == 1);

    public async Task AddAsync(InventorySettings settings) =>
        await context.InventorySettings.AddAsync(settings);

    public async Task SaveChangesAsync() => await context.SaveChangesAsync();
}
