using MiniStore.Domain.Entities;

namespace MiniStore.Domain.Interfaces;

public interface IPurchaseApprovalRepository
{
    Task<List<PurchaseApprovalRule>> GetRulesAsync();
    Task<PurchaseApprovalRule?> GetRuleAsync(int id);
    Task<PurchaseApprovalRule?> ResolveRuleAsync(int warehouseId, PurchaseRequestPriority priority);
    Task<PurchaseApprovalInstance?> GetInstanceAsync(int purchaseRequestId);
    Task AddRuleAsync(PurchaseApprovalRule rule);
    Task AddInstanceAsync(PurchaseApprovalInstance instance);
    Task SaveChangesAsync();
}
