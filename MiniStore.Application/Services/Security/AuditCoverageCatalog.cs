using MiniStore.Domain.Entities;

namespace MiniStore.Application.Services;

public enum AuditEvidenceKind
{
    SemanticAuditEvent = 1,
    AuthoritativeDomainHistory = 2
}

public sealed record CriticalAuditCoverageControl(
    string ControlCode,
    AuditEvidenceKind EvidenceKind,
    string EvidenceReference,
    string ExpectedScope,
    AuditEventOutcome ExpectedOutcome,
    string? EventType = null);

public static class AuditCoverageCatalog
{
    public static IReadOnlyList<CriticalAuditCoverageControl> Controls { get; } =
    [
        Event("SEC-ROLE-CREATE", SemanticAuditEventTypes.TenantRoleCreated, "TenantRoleService.CreateAsync", "Tenant", AuditEventOutcome.Succeeded),
        Event("SEC-ROLE-UPDATE", SemanticAuditEventTypes.TenantRoleUpdated, "TenantRoleService.UpdateAsync", "Tenant", AuditEventOutcome.Succeeded),
        Event("SEC-ROLE-PERMISSIONS", SemanticAuditEventTypes.TenantRolePermissionsChanged, "TenantRoleService.UpdateAsync", "Tenant", AuditEventOutcome.Succeeded),
        Event("SEC-ROLE-DELETE", SemanticAuditEventTypes.TenantRoleDeleted, "TenantRoleService.DeleteAsync", "Tenant", AuditEventOutcome.Succeeded),
        Event("SEC-USER-CREATE", SemanticAuditEventTypes.TenantUserCreated, "TenantUserAdministrationService.CreateAsync", "Tenant", AuditEventOutcome.Succeeded),
        Event("CFG-ACCOUNTING", SemanticAuditEventTypes.AccountingSettingsChanged, "AccountingSettingsService.UpdateAsync", "Tenant", AuditEventOutcome.Succeeded),
        Event("CFG-GENERAL", SemanticAuditEventTypes.GeneralSettingsChanged, "GeneralSettingsService.UpdateAsync", "Tenant", AuditEventOutcome.Succeeded),
        Event("CFG-DISCOUNT", SemanticAuditEventTypes.DiscountSettingsChanged, "DiscountSettingsService.UpdateAsync", "Tenant", AuditEventOutcome.Succeeded),
        Event("CFG-INVENTORY", SemanticAuditEventTypes.InventorySettingsChanged, "InventorySettingsService.UpdateAsync", "Tenant", AuditEventOutcome.Succeeded),
        Event("CFG-PURCHASE-MATCHING", SemanticAuditEventTypes.PurchaseMatchingSettingsChanged, "PurchaseMatchingSettingsService.UpdateAsync", "Tenant", AuditEventOutcome.Succeeded),
        Event("CFG-PURCHASE-SOURCING", SemanticAuditEventTypes.PurchaseSourcingPolicyChanged, "PurchaseSourcingPolicyService.UpdateAsync", "Tenant", AuditEventOutcome.Succeeded),
        Event("SEC-TENANT-LOGIN-DENIED", SemanticAuditEventTypes.TenantLoginFailed, "AccountController.Login", "Tenant", AuditEventOutcome.Denied),
        Event("SEC-PLATFORM-LOGIN-DENIED", SemanticAuditEventTypes.PlatformLoginFailed, "PlatformAccountController.Login", "Platform", AuditEventOutcome.Denied),
        Event("SEC-PRIVILEGED-DENIED", SemanticAuditEventTypes.PrivilegedActionDenied, "AuditingAuthorizationMiddlewareResultHandler", "TenantOrPlatform", AuditEventOutcome.Denied),
        Event("ADM-SUBSCRIPTION", SemanticAuditEventTypes.PlatformCompanySubscriptionChanged, "PlatformSaasService.UpdateCompanySubscriptionAsync", "Platform", AuditEventOutcome.Succeeded),
        Event("ADM-PAYMENT", SemanticAuditEventTypes.PlatformPaymentConfirmed, "BillingCheckoutService.ConfirmAsync", "Platform", AuditEventOutcome.Succeeded),
        Event("ADM-PLAN", SemanticAuditEventTypes.PlatformPlanCreated, "PlatformSaasService.CreatePlanAsync", "Platform", AuditEventOutcome.Succeeded),
        Event("ADM-PROMOTION", SemanticAuditEventTypes.PlatformPromotionCreated, "PlatformSaasService.CreatePromotionAsync", "Platform", AuditEventOutcome.Succeeded),
        Event("INT-CREDENTIAL-CREATE", SemanticAuditEventTypes.SupplierCredentialCreated, "SupplierIntegrationService.CreateCredentialAsync", "Tenant", AuditEventOutcome.Succeeded),
        Event("INT-CREDENTIAL-DEACTIVATE", SemanticAuditEventTypes.SupplierCredentialDeactivated, "SupplierIntegrationService.DeactivateCredentialAsync", "Tenant", AuditEventOutcome.Succeeded),
        Event("INT-PORTAL-CREATE", SemanticAuditEventTypes.SupplierPortalAccessCreated, "SupplierIntegrationService.CreatePortalAccessAsync", "Tenant", AuditEventOutcome.Succeeded),
        Event("INT-PORTAL-REVOKE", SemanticAuditEventTypes.SupplierPortalAccessRevoked, "SupplierIntegrationService.RevokePortalAccessAsync", "Tenant", AuditEventOutcome.Succeeded),
        Event("INT-API-AUTH-DENIED", SemanticAuditEventTypes.SupplierApiAuthenticationDenied, "SupplierIntegrationApiController", "Tenant", AuditEventOutcome.Denied),
        Event("INT-PORTAL-AUTH-DENIED", SemanticAuditEventTypes.SupplierPortalAuthenticationDenied, "SupplierPortalController", "Tenant", AuditEventOutcome.Denied),
        Event("AUD-EXPORT", SemanticAuditEventTypes.AuditEventsExported, "AuditEventReadService.ExportAsync", "TenantOrPlatform", AuditEventOutcome.Succeeded),
        Event("AUD-HOLD-CREATE", SemanticAuditEventTypes.AuditLegalHoldCreated, "AuditRetentionService.CreateLegalHoldAsync", "Platform", AuditEventOutcome.Succeeded),
        Event("AUD-HOLD-RELEASE", SemanticAuditEventTypes.AuditLegalHoldReleased, "AuditRetentionService.ReleaseLegalHoldAsync", "Platform", AuditEventOutcome.Succeeded),
        Event("AUD-CHECKPOINT", SemanticAuditEventTypes.AuditCheckpointCreated, "AuditRetentionService.CreateCheckpointAsync", "System", AuditEventOutcome.Succeeded),
        Event("AUD-RETENTION", SemanticAuditEventTypes.AuditRetentionExecuted, "AuditRetentionService.RunRetentionAsync", "System", AuditEventOutcome.Succeeded),

        Domain("ACC-POST-REVERSE", "JournalEntry + source identity + reversal link", "Tenant", AuditEventOutcome.Succeeded),
        Domain("INV-MOVEMENT-LIFECYCLE", "StockMovement + LocationMovement + InventoryTrackingTransaction", "Tenant", AuditEventOutcome.Succeeded),
        Domain("INV-TRANSFER-LIFECYCLE", "StockTransferHistory + physical StockMovement", "Tenant", AuditEventOutcome.Succeeded),
        Domain("PUR-REQUEST-DECISIONS", "PurchaseRequestHistory + PurchaseApprovalStep", "Tenant", AuditEventOutcome.Succeeded),
        Domain("PUR-ORDER-LIFECYCLE", "PurchaseOrderLifecycleHistory + supplier confirmation evidence", "Tenant", AuditEventOutcome.Succeeded),
        Domain("PUR-RECEIPT-BILL-PAYMENT", "GoodsReceipt + VendorBill + PurchaseMatchException + SupplierPayment", "Tenant", AuditEventOutcome.Succeeded),
        Domain("PUR-REVERSAL", "PurchaseDocumentReversal + original/reversal accounting references", "Tenant", AuditEventOutcome.Succeeded),
        Domain("SALES-POST-RETURN", "Sale + SalesReturn + JournalEntry source identity", "Tenant", AuditEventOutcome.Succeeded),
        Domain("INT-SUPPLIER-ACTIVITY", "SupplierExternalAuditEvent + idempotent external submission", "Tenant", AuditEventOutcome.Succeeded)
    ];

    public static IReadOnlySet<string> SemanticEventTypes { get; } = Controls
        .Where(control => control.EvidenceKind == AuditEvidenceKind.SemanticAuditEvent)
        .Select(control => control.EventType!)
        .ToHashSet(StringComparer.Ordinal);

    public static void RequireRegisteredSemanticEvent(string eventType)
    {
        if (!SemanticEventTypes.Contains(eventType))
            throw new InvalidOperationException($"Semantic audit event '{eventType}' is missing from the critical coverage catalog.");
    }

    private static CriticalAuditCoverageControl Event(
        string code,
        string eventType,
        string reference,
        string scope,
        AuditEventOutcome outcome) =>
        new(code, AuditEvidenceKind.SemanticAuditEvent, reference, scope, outcome, eventType);

    private static CriticalAuditCoverageControl Domain(
        string code,
        string reference,
        string scope,
        AuditEventOutcome outcome) =>
        new(code, AuditEvidenceKind.AuthoritativeDomainHistory, reference, scope, outcome);
}
