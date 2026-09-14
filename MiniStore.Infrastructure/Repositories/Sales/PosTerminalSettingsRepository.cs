using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;

namespace MiniStore.Infrastructure.Repositories;

public class PosTerminalSettingsRepository(AppDbContext context)
    : IPosTerminalSettingsRepository
{
    public Task<List<PosTerminalSettings>> GetAllAsync() =>
        context.PosTerminalSettings.OrderBy(x => x.PosTerminalId).ToListAsync();

    public Task<PosTerminalSettings?> GetAsync(int posTerminalId) =>
        context.PosTerminalSettings.FirstOrDefaultAsync(x => x.PosTerminalId == posTerminalId);

    public Task AddAsync(PosTerminalSettings settings) =>
        context.PosTerminalSettings.AddAsync(settings).AsTask();

    public Task SaveChangesAsync() => context.SaveChangesAsync();
}
