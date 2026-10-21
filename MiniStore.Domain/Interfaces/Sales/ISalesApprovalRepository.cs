using MiniStore.Domain.Entities;

namespace MiniStore.Domain.Interfaces;

public interface ISalesApprovalRepository
{
    Task<List<SalesApprovalRule>> GetRulesAsync();
    Task<SalesApprovalRule?> GetRuleAsync(int id);
    Task AddRuleAsync(SalesApprovalRule rule);
    Task<SalesOrderApproval?> GetLatestReviewAsync(int salesOrderId);
    Task<SalesOrderApproval?> GetPendingReviewAsync(int salesOrderId);
    Task AddReviewAsync(SalesOrderApproval review);
}
