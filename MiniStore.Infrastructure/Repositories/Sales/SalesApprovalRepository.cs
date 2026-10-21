using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;

namespace MiniStore.Infrastructure.Repositories;

public sealed class SalesApprovalRepository(AppDbContext context) : ISalesApprovalRepository
{
    private IQueryable<SalesApprovalRule> Rules(bool tracking = false)
    {
        var query = context.SalesApprovalRules.Include(x => x.Steps).AsQueryable();
        return tracking ? query : query.AsNoTracking();
    }

    private IQueryable<SalesOrderApproval> Reviews(bool tracking = false)
    {
        var query = context.SalesOrderApprovals.Include(x => x.Findings).Include(x => x.Steps).AsQueryable();
        return tracking ? query : query.AsNoTracking();
    }

    public Task<List<SalesApprovalRule>> GetRulesAsync() => Rules().OrderBy(x => x.Priority).ThenBy(x => x.Id).ToListAsync();
    public Task<SalesApprovalRule?> GetRuleAsync(int id) => Rules(true).SingleOrDefaultAsync(x => x.Id == id);
    public Task AddRuleAsync(SalesApprovalRule rule) => context.SalesApprovalRules.AddAsync(rule).AsTask();
    public Task<SalesOrderApproval?> GetLatestReviewAsync(int salesOrderId) => Reviews().Where(x => x.SalesOrderId == salesOrderId)
        .OrderByDescending(x => x.RequestedAtUtc).ThenByDescending(x => x.Id).FirstOrDefaultAsync();
    public Task<SalesOrderApproval?> GetPendingReviewAsync(int salesOrderId) => Reviews(true)
        .SingleOrDefaultAsync(x => x.SalesOrderId == salesOrderId && x.Status == SalesOrderApprovalStatus.Pending);
    public Task AddReviewAsync(SalesOrderApproval review) => context.SalesOrderApprovals.AddAsync(review).AsTask();
}
