using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;
namespace MiniStore.Infrastructure.Repositories;
public sealed class PurchaseMatchRepository(AppDbContext context) : IPurchaseMatchRepository
{
    public Task<PurchaseMatchRun?> GetByVendorBillIdAsync(int vendorBillId) => context.PurchaseMatchRuns.Include(x => x.Exceptions).SingleOrDefaultAsync(x => x.VendorBillId == vendorBillId);
    public Task AddAsync(PurchaseMatchRun run) => context.PurchaseMatchRuns.AddAsync(run).AsTask();
}
public sealed class PurchaseMatchingSettingsRepository(AppDbContext context) : IPurchaseMatchingSettingsRepository
{
    public Task<PurchaseMatchingSettings?> GetAsync() => context.PurchaseMatchingSettings.SingleOrDefaultAsync(x => x.SingletonKey == 1);
    public Task AddAsync(PurchaseMatchingSettings settings) => context.PurchaseMatchingSettings.AddAsync(settings).AsTask();
    public Task SaveChangesAsync() => context.SaveChangesAsync();
}
