using MiniStore.Domain.Entities;

namespace MiniStore.Application.Services;

public static class SemanticAuditEventTypes
{
    public const string TenantRoleCreated = "Security.TenantRoleCreated";
    public const string TenantRoleUpdated = "Security.TenantRoleUpdated";
    public const string TenantRolePermissionsChanged = "Security.TenantRolePermissionsChanged";
    public const string TenantRoleDeleted = "Security.TenantRoleDeleted";
    public const string AccountingSettingsChanged = "Configuration.AccountingSettingsChanged";
    public const string GeneralSettingsChanged = "Configuration.GeneralSettingsChanged";
    public const string DiscountSettingsChanged = "Configuration.DiscountSettingsChanged";
    public const string InventorySettingsChanged = "Configuration.InventorySettingsChanged";
    public const string PurchaseMatchingSettingsChanged = "Configuration.PurchaseMatchingSettingsChanged";
    public const string PurchaseSourcingPolicyChanged = "Configuration.PurchaseSourcingPolicyChanged";
}

public sealed class SemanticAuditEventService(
    IAuditEventWriter writer,
    IAuditRequestContext requestContext)
{
    public ValueTask<Guid?> RecordTenantSuccessAsync(
        AuditEventCategory category,
        string eventType,
        string action,
        string resourceType,
        string resourceId,
        IReadOnlyDictionary<string, object?>? beforeValues = null,
        IReadOnlyDictionary<string, object?>? afterValues = null,
        IReadOnlyDictionary<string, object?>? metadata = null,
        string? operationId = null,
        CancellationToken cancellationToken = default) =>
        writer.RecordAsync(new AuditEventWriteRequest
        {
            Scope = AuditEventScope.Tenant,
            Category = category,
            Severity = AuditEventSeverity.Information,
            Outcome = AuditEventOutcome.Succeeded,
            EventType = eventType,
            Action = action,
            ActorType = AuditActorType.User,
            SourceChannel = requestContext.SourceChannel,
            CorrelationId = requestContext.CorrelationId,
            OperationId = operationId,
            ResourceType = resourceType,
            ResourceId = resourceId,
            BeforeValues = beforeValues,
            AfterValues = afterValues,
            Metadata = metadata,
            RequestPathTemplate = requestContext.RequestPathTemplate
        }, cancellationToken);
}
