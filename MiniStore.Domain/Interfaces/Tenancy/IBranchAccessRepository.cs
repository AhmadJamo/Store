using MiniStore.Domain.Entities;

namespace MiniStore.Domain.Interfaces;

public sealed record BranchAccessUser(string Id, string DisplayName, string? Email);

public interface IBranchAccessRepository
{
    Task<IReadOnlyList<int>> GetGrantedBranchIdsAsync(int tenantId, string userId);
    Task<IReadOnlyList<TenantUserBranchGrant>> GetAllGrantsAsync(int tenantId);
    Task<IReadOnlyList<BranchAccessUser>> GetActiveUsersAsync(int tenantId);
    Task ReplaceUserGrantsAsync(int tenantId, string userId, IReadOnlyCollection<int> branchIds, string grantedByUserId);
}
