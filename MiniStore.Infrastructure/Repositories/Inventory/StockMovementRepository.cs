using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;

namespace MiniStore.Infrastructure.Repositories;

public sealed class StockMovementRepository(AppDbContext context) : IStockMovementRepository
{
    public Task<List<StockMovement>> GetAllAsync() => context.StockMovements.AsNoTracking()
        .OrderByDescending(x => x.PostedAt).ThenByDescending(x => x.Id).ToListAsync();
    public Task<StockMovement?> GetByIdempotencyKeyAsync(string key) =>
        context.StockMovements.FirstOrDefaultAsync(x => x.IdempotencyKey == key);
    public Task AddAsync(StockMovement movement) => context.StockMovements.AddAsync(movement).AsTask();
}
