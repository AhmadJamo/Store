using MiniStore.Application.Dtos.Security;
using MiniStore.Application.Permissions;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public sealed class BranchAccessService(
    IBranchAccessRepository repository,
    IBranchRepository branches,
    ITenantContext tenantContext,
    ICurrentUserService currentUser,
    IPermissionService permissions,
    IUnitOfWork unitOfWork,
    SemanticAuditEventService audit)
{
    public async Task<IReadOnlySet<int>> GetAccessibleBranchIdsAsync()
    {
        var tenantId = RequireTenant();
        if (await permissions.CanAsync(currentUser.UserId, "Branches.ManageAccess"))
            return (await branches.GetAllAsync()).Select(x => x.Id).ToHashSet();
        return (await repository.GetGrantedBranchIdsAsync(tenantId, currentUser.UserId)).ToHashSet();
    }

    public async Task<bool> CanAccessAsync(int branchId)
    {
        if (branchId <= 0) return false;
        var branch = await branches.GetByIdAsync(branchId);
        return branch?.IsActive == true &&
               (await GetAccessibleBranchIdsAsync()).Contains(branchId);
    }

    public async Task<BranchAccessManagementDto> GetManagementAsync()
    {
        var tenantId = RequireTenant();
        var branchRows = await branches.GetAllAsync();
        var users = await repository.GetActiveUsersAsync(tenantId);
        var grants = await repository.GetAllGrantsAsync(tenantId);
        var byUser = grants.GroupBy(x => x.UserId)
            .ToDictionary(x => x.Key, x => (IReadOnlyList<int>)x.Select(y => y.BranchId).Order().ToList());
        return new BranchAccessManagementDto(
            branchRows.OrderBy(x => x.Code)
                .Select(x => new BranchAccessOptionDto(x.Id, x.Code, x.Name, x.IsActive)).ToList(),
            users.Select(x => new BranchAccessUserDto(
                x.Id, x.DisplayName, x.Email, byUser.GetValueOrDefault(x.Id) ?? [])).ToList());
    }

    public async Task<BranchAccessManagementDto> ReplaceUserGrantsAsync(ReplaceUserBranchGrantsDto dto)
    {
        var tenantId = RequireTenant();
        var branchIds = dto.BranchIds.Distinct().ToArray();
        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            await repository.ReplaceUserGrantsAsync(
                tenantId, dto.UserId.Trim(), branchIds, currentUser.UserId);
            await audit.RecordTenantSuccessAsync(
                AuditEventCategory.Security,
                SemanticAuditEventTypes.TenantUserBranchGrantsChanged,
                "Replace",
                "TenantUserBranchGrant",
                dto.UserId.Trim(),
                afterValues: new Dictionary<string, object?>
                {
                    ["BranchCount"] = branchIds.Length,
                    ["BranchFingerprint"] = string.Join(",", branchIds.Order())
                });
        });
        return await GetManagementAsync();
    }

    private int RequireTenant() => tenantContext.TenantId
        ?? throw new InvalidOperationException("An active company is required.");
}
