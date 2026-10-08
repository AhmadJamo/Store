using MiniStore.Application.DTOs.Purchases;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public sealed class PurchaseApprovalService(
    IPurchaseApprovalRepository approvals,
    IPurchaseRequestRepository requests,
    IWarehouseRepository warehouses,
    ITenantRoleRepository roles,
    ITenantAuthorizationRepository authorization,
    ITenantContext tenantContext,
    ICurrentUserService currentUser,
    IUnitOfWork unitOfWork)
{
    public async Task<PurchaseApprovalRulesPageDto> GetRulesPageAsync(CreatePurchaseApprovalRuleDto? form = null)
    {
        var warehouseNames = (await warehouses.GetAllAsync()).ToDictionary(x => x.Id, x => x.Name);
        return new PurchaseApprovalRulesPageDto
        {
            Form = form ?? new(),
            Warehouses = warehouseNames.Select(x => new PurchaseRequestOptionDto(x.Key, x.Value)).ToList(),
            Roles = (await roles.GetAllAsync(RequireTenant())).Select(x => x.Name).OrderBy(x => x).ToList(),
            Rules = (await approvals.GetRulesAsync()).Select(x => new PurchaseApprovalRuleDto
            {
                Id = x.Id,
                Name = x.Name,
                WarehouseName = x.WarehouseId.HasValue
                    ? warehouseNames.GetValueOrDefault(x.WarehouseId.Value, $"#{x.WarehouseId}") : "All warehouses",
                MinimumPriority = x.MinimumPriority,
                MaximumPriority = x.MaximumPriority,
                IsActive = x.IsActive,
                RowVersion = x.RowVersion,
                ApproverRoles = x.Steps.OrderBy(s => s.Sequence).Select(s => s.ApproverRoleName).ToList()
            }).ToList()
        };
    }

    public async Task CreateRuleAsync(CreatePurchaseApprovalRuleDto dto)
    {
        if (dto.WarehouseId.HasValue && await warehouses.GetByIdAsync(dto.WarehouseId.Value) is null)
            throw new ArgumentException("Warehouse was not found.");
        var roleNames = dto.ApproverRoles.Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim()).ToList();
        if (roleNames.Count == 0) throw new ArgumentException("At least one approver role is required.");
        if (roleNames.Distinct(StringComparer.OrdinalIgnoreCase).Count() != roleNames.Count)
            throw new ArgumentException("Each approver role can appear only once in a rule.");
        var availableRoles = (await roles.GetAllAsync(RequireTenant())).Select(x => x.Name)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        if (roleNames.Any(x => !availableRoles.Contains(x)))
            throw new ArgumentException("One or more approver roles are invalid.");
        var rule = new PurchaseApprovalRule(dto.Name, dto.WarehouseId, dto.MinimumPriority, dto.MaximumPriority);
        for (var index = 0; index < roleNames.Count; index++) rule.AddStep(index + 1, roleNames[index]);
        await approvals.AddRuleAsync(rule);
        await approvals.SaveChangesAsync();
    }

    public async Task SetActiveAsync(int id, bool active, byte[] rowVersion)
    {
        var rule = await approvals.GetRuleAsync(id) ?? throw new InvalidOperationException("Approval rule was not found.");
        if (!rule.RowVersion.SequenceEqual(rowVersion))
            throw new InvalidOperationException("Approval rule was modified by another user. Reload and try again.");
        rule.SetActive(active);
        await approvals.SaveChangesAsync();
    }

    public async Task SubmitAsync(PurchaseRequest request)
    {
        var rule = await approvals.ResolveRuleAsync(request.WarehouseId, request.Priority)
            ?? throw new InvalidOperationException("No active approval rule matches this purchase request.");
        if (rule.Steps.Count == 0) throw new InvalidOperationException("The matching approval rule has no steps.");
        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            request.Submit(currentUser.UserId);
            await approvals.AddInstanceAsync(new PurchaseApprovalInstance(
                request.Id, rule.Id, rule.Name, rule.Steps, currentUser.UserId));
        });
    }

    public async Task<PurchaseApprovalInstanceDto?> GetInstanceAsync(int purchaseRequestId)
    {
        var instance = await approvals.GetInstanceAsync(purchaseRequestId);
        if (instance is null) return null;
        var currentRole = instance.Status == PurchaseApprovalStatus.Pending
            ? instance.CurrentStep().ApproverRoleName : null;
        return new PurchaseApprovalInstanceDto
        {
            Status = instance.Status,
            RuleName = instance.RuleNameSnapshot,
            CurrentRole = currentRole,
            CanCurrentUserDecide = currentRole is not null &&
                await authorization.HasRoleAsync(RequireTenant(), currentUser.UserId, currentRole),
            Steps = instance.Steps.OrderBy(x => x.Sequence).Select(x => new PurchaseApprovalStepDto
            {
                Sequence = x.Sequence, RoleName = x.ApproverRoleName, Status = x.Status,
                DecidedByUserId = x.DecidedByUserId, DecidedAtUtc = x.DecidedAtUtc, Note = x.Note
            }).ToList()
        };
    }

    public async Task DecideAsync(int requestId, bool approve, string? note)
    {
        var request = await requests.GetByIdAsync(requestId)
            ?? throw new InvalidOperationException("Purchase request was not found.");
        var instance = await approvals.GetInstanceAsync(requestId)
            ?? throw new InvalidOperationException("Purchase approval was not found.");
        var requiredRole = instance.CurrentStep().ApproverRoleName;
        if (!await authorization.HasRoleAsync(RequireTenant(), currentUser.UserId, requiredRole))
            throw new UnauthorizedAccessException("The current approval step requires a different role.");
        await unitOfWork.ExecuteInTransactionAsync(() =>
        {
            if (approve)
            {
                instance.ApproveCurrent(currentUser.UserId, note);
                if (instance.Status == PurchaseApprovalStatus.Approved) request.Approve(currentUser.UserId);
            }
            else
            {
                instance.RejectCurrent(currentUser.UserId, note ?? string.Empty);
                request.Reject(currentUser.UserId, note ?? string.Empty);
            }
            return Task.CompletedTask;
        });
    }

    public async Task CancelPendingAsync(int requestId)
    {
        var instance = await approvals.GetInstanceAsync(requestId);
        instance?.Cancel();
    }

    private int RequireTenant() => tenantContext.TenantId
        ?? throw new InvalidOperationException("An active company is required.");
}
