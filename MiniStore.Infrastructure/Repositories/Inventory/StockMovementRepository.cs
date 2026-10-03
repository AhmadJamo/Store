using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;

namespace MiniStore.Infrastructure.Repositories;

public sealed class StockMovementRepository(AppDbContext context) : IStockMovementRepository
{
    public Task<List<StockMovement>> GetAllAsync() => context.StockMovements.AsNoTracking()
        .OrderByDescending(x => x.CreatedAt).ThenByDescending(x => x.Id).ToListAsync();
    public Task<StockMovement?> GetByIdempotencyKeyAsync(string key) =>
        context.StockMovements.FirstOrDefaultAsync(x => x.IdempotencyKey == key);
    public Task<List<StockMovement>> GetBySourceAsync(string type, int id) => context.StockMovements
        .Where(x => x.SourceDocumentType == type && x.SourceDocumentId == id)
        .OrderBy(x => x.SourceLineId).ThenBy(x => x.StageSequence).ToListAsync();
    public Task AddAsync(StockMovement movement) => context.StockMovements.AddAsync(movement).AsTask();
}
