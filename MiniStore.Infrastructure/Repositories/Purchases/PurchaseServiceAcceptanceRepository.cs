using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;

namespace MiniStore.Infrastructure.Repositories;

public sealed class PurchaseServiceAcceptanceRepository(AppDbContext context) : IPurchaseServiceAcceptanceRepository
{
    public Task<List<PurchaseServiceAcceptance>> GetAllAsync() => context.PurchaseServiceAcceptances.Include(x => x.Lines).OrderByDescending(x => x.CompletedAtUtc).ToListAsync();
    public Task<List<PurchaseServiceAcceptance>> GetByPurchaseOrderIdAsync(int purchaseOrderId) => context.PurchaseServiceAcceptances.Include(x => x.Lines).Where(x => x.PurchaseOrderId == purchaseOrderId).ToListAsync();
    public Task<PurchaseServiceAcceptance?> GetByIdAsync(int id) => context.PurchaseServiceAcceptances.Include(x => x.Lines).FirstOrDefaultAsync(x => x.Id == id);
    public Task AddAsync(PurchaseServiceAcceptance acceptance) => context.PurchaseServiceAcceptances.AddAsync(acceptance).AsTask();
}
