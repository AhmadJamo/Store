using MiniStore.Domain.Entities;

namespace MiniStore.Application.Services;

public static class SemanticAuditEventTypes
{
    public const string TenantRoleCreated = "Security.TenantRoleCreated";
    public const string TenantRoleUpdated = "Security.TenantRoleUpdated";
    public const string TenantRolePermissionsChanged = "Security.TenantRolePermissionsChanged";
    public const string TenantRoleDeleted = "Security.TenantRoleDeleted";
    public const string TenantUserCreated = "Security.TenantUserCreated";
    public const string AccountingSettingsChanged = "Configuration.AccountingSettingsChanged";
    public const string GeneralSettingsChanged = "Configuration.GeneralSettingsChanged";
    public const string DiscountSettingsChanged = "Configuration.DiscountSettingsChanged";
    public const string InventorySettingsChanged = "Configuration.InventorySettingsChanged";
    public const string PurchaseMatchingSettingsChanged = "Configuration.PurchaseMatchingSettingsChanged";
    public const string PurchaseSourcingPolicyChanged = "Configuration.PurchaseSourcingPolicyChanged";
    public const string TenantLoginFailed = "Security.TenantLoginFailed";
    public const string PlatformLoginFailed = "Security.PlatformLoginFailed";
    public const string PrivilegedActionDenied = "Security.PrivilegedActionDenied";
    public const string PlatformCompanySubscriptionChanged = "Administration.PlatformCompanySubscriptionChanged";
    public const string PlatformPaymentConfirmed = "Administration.PlatformPaymentConfirmed";
    public const string PlatformPlanCreated = "Administration.PlatformPlanCreated";
    public const string PlatformPromotionCreated = "Administration.PlatformPromotionCreated";
    public const string SupplierCredentialCreated = "Integration.SupplierCredentialCreated";
    public const string SupplierCredentialDeactivated = "Integration.SupplierCredentialDeactivated";
    public const string SupplierPortalAccessCreated = "Integration.SupplierPortalAccessCreated";
    public const string SupplierPortalAccessRevoked = "Integration.SupplierPortalAccessRevoked";
    public const string SupplierApiAuthenticationDenied = "Integration.SupplierApiAuthenticationDenied";
    public const string SupplierPortalAuthenticationDenied = "Integration.SupplierPortalAuthenticationDenied";
    public const string AuditEventsExported = "DataAccess.AuditEventsExported";
    public const string AuditLegalHoldCreated = "Administration.AuditLegalHoldCreated";
    public const string AuditLegalHoldReleased = "Administration.AuditLegalHoldReleased";
    public const string AuditCheckpointCreated = "Security.AuditCheckpointCreated";
    public const string AuditRetentionExecuted = "Security.AuditRetentionExecuted";
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
            RequestPathTemplate = requestContext.RequestPathTemplate,
            SourceIpMasked = requestContext.SourceIpMasked,
            UserAgentHash = requestContext.UserAgentHash
        }, cancellationToken);

    public ValueTask<Guid?> RecordPlatformSuccessAsync(
        string eventType,
        string action,
        string resourceType,
        string resourceId,
        CancellationToken cancellationToken = default) =>
        writer.RecordAsync(new AuditEventWriteRequest
        {
            Scope = AuditEventScope.Platform,
            Category = AuditEventCategory.Administration,
            Severity = AuditEventSeverity.Information,
            Outcome = AuditEventOutcome.Succeeded,
            EventType = eventType,
            Action = action,
            ActorType = AuditActorType.User,
            SourceChannel = AuditSourceChannel.Platform,
            CorrelationId = requestContext.CorrelationId,
            ResourceType = resourceType,
            ResourceId = resourceId,
            RequestPathTemplate = requestContext.RequestPathTemplate,
            SourceIpMasked = requestContext.SourceIpMasked,
            UserAgentHash = requestContext.UserAgentHash
        }, cancellationToken);
}
