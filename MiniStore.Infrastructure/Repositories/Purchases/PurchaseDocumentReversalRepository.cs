using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;

namespace MiniStore.Infrastructure.Repositories;

public sealed class PurchaseDocumentReversalRepository(AppDbContext context) : IPurchaseDocumentReversalRepository
{
    public Task<List<PurchaseDocumentReversal>> GetAllAsync() => context.PurchaseDocumentReversals.AsNoTracking()
        .OrderByDescending(x => x.ReversalDate).ThenByDescending(x => x.Id).ToListAsync();
    public Task<PurchaseDocumentReversal?> GetByIdAsync(int id) => context.PurchaseDocumentReversals.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id);
    public Task<PurchaseDocumentReversal?> GetByRequestKeyAsync(string key) => context.PurchaseDocumentReversals.SingleOrDefaultAsync(x => x.IdempotencyKey == key);
    public Task<PurchaseDocumentReversal?> GetByOriginalAsync(PurchaseDocumentType type, int originalDocumentId) =>
        context.PurchaseDocumentReversals.SingleOrDefaultAsync(x => x.DocumentType == type && x.OriginalDocumentId == originalDocumentId);
    public Task AddAsync(PurchaseDocumentReversal value) => context.PurchaseDocumentReversals.AddAsync(value).AsTask();
}
