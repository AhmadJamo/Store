using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;

namespace MiniStore.Infrastructure.Repositories;

public sealed class PurchaseApprovalRepository(AppDbContext context) : IPurchaseApprovalRepository
{
    public Task<List<PurchaseApprovalRule>> GetRulesAsync() => context.PurchaseApprovalRules
        .Include(x => x.Steps).OrderBy(x => x.Name).ToListAsync();

    public Task<PurchaseApprovalRule?> GetRuleAsync(int id) => context.PurchaseApprovalRules
        .Include(x => x.Steps).SingleOrDefaultAsync(x => x.Id == id);

    public Task<PurchaseApprovalRule?> ResolveRuleAsync(int warehouseId, PurchaseRequestPriority priority) =>
        context.PurchaseApprovalRules.Include(x => x.Steps)
            .Where(x => x.IsActive && (!x.WarehouseId.HasValue || x.WarehouseId == warehouseId) &&
                        priority >= x.MinimumPriority && priority <= x.MaximumPriority)
            .OrderByDescending(x => x.WarehouseId.HasValue)
            .ThenBy(x => (int)x.MaximumPriority - (int)x.MinimumPriority)
            .ThenBy(x => x.Id)
            .FirstOrDefaultAsync();

    public Task<PurchaseApprovalInstance?> GetInstanceAsync(int purchaseRequestId) =>
        context.PurchaseApprovalInstances.Include(x => x.Steps)
            .SingleOrDefaultAsync(x => x.PurchaseRequestId == purchaseRequestId);

    public Task AddRuleAsync(PurchaseApprovalRule rule) => context.PurchaseApprovalRules.AddAsync(rule).AsTask();
    public Task AddInstanceAsync(PurchaseApprovalInstance instance) =>
        context.PurchaseApprovalInstances.AddAsync(instance).AsTask();
    public Task SaveChangesAsync() => context.SaveChangesAsync();
}
