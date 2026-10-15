using MiniStore.Application.Dtos.Settings;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public sealed class PurchaseSourcingPolicyService(
    IPurchaseSourcingPolicyRepository repository,
    ITenantRoleRepository roles,
    ITenantContext tenantContext,
    GeneralSettingsService generalSettings,
    SemanticAuditEventService audit)
{
    public async Task<PurchaseSourcingPolicyDto> GetAsync()
    {
        var policy = await repository.GetAsync();
        var settings = await generalSettings.GetAsync();
        return new PurchaseSourcingPolicyDto
        {
            AllowDirectRfq = policy?.AllowDirectRfq ?? false,
            MaximumEstimatedAmountCompanyCurrency = policy?.MaximumEstimatedAmountCompanyCurrency ?? 0,
            AllowedRoleName = policy?.AllowedRoleName,
            RequireActiveSuppliers = policy?.RequireActiveSuppliers ?? true,
            RowVersion = policy?.RowVersion ?? [],
            AvailableRoles = (await roles.GetAllAsync(RequireTenant())).Select(x => x.Name).OrderBy(x => x).ToList(),
            CompanyCurrencyCode = settings.Currency
        };
    }

    public Task<PurchaseSourcingPolicy?> GetPolicyAsync() => repository.GetAsync();

    public async Task UpdateAsync(PurchaseSourcingPolicyDto dto)
    {
        var availableRoles = (await roles.GetAllAsync(RequireTenant())).Select(x => x.Name)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        if (!string.IsNullOrWhiteSpace(dto.AllowedRoleName) && !availableRoles.Contains(dto.AllowedRoleName.Trim()))
            throw new ArgumentException("The selected direct RFQ role was not found.");
        var policy = await repository.GetAsync();
        var before = policy is null ? null : AuditValues(
            policy.AllowDirectRfq,
            policy.MaximumEstimatedAmountCompanyCurrency,
            policy.AllowedRoleName,
            policy.RequireActiveSuppliers);
        if (policy is null)
            await repository.AddAsync(new PurchaseSourcingPolicy(dto.AllowDirectRfq,
                dto.MaximumEstimatedAmountCompanyCurrency, dto.AllowedRoleName, dto.RequireActiveSuppliers));
        else
        {
            if (!policy.RowVersion.SequenceEqual(dto.RowVersion))
                throw new InvalidOperationException("Purchase sourcing policy was modified by another user. Reload and try again.");
            policy.Configure(dto.AllowDirectRfq, dto.MaximumEstimatedAmountCompanyCurrency,
                dto.AllowedRoleName, dto.RequireActiveSuppliers);
        }
        await audit.RecordTenantSuccessAsync(
            AuditEventCategory.Configuration,
            SemanticAuditEventTypes.PurchaseSourcingPolicyChanged,
            "Update",
            "PurchaseSourcingPolicy",
            "Current",
            before,
            AuditValues(dto.AllowDirectRfq, dto.MaximumEstimatedAmountCompanyCurrency,
                dto.AllowedRoleName, dto.RequireActiveSuppliers));
        await repository.SaveChangesAsync();
    }

    private static IReadOnlyDictionary<string, object?> AuditValues(
        bool allowDirectRfq,
        decimal maximumAmount,
        string? allowedRoleName,
        bool requireActiveSuppliers) => new Dictionary<string, object?>
    {
        ["AllowDirectRfq"] = allowDirectRfq,
        ["MaximumEstimatedAmountCompanyCurrency"] = maximumAmount,
        ["AllowedRoleName"] = allowedRoleName,
        ["RequireActiveSuppliers"] = requireActiveSuppliers
    };

    private int RequireTenant() => tenantContext.TenantId
        ?? throw new InvalidOperationException("An active company is required.");
}
