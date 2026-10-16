namespace MiniStore.Web.Authorization;

public static class PlatformAuthentication
{
    public const string Scheme = "PlatformCookie";
    public const string Policy = "PlatformAccess";
    public const string ManagementPolicy = "PlatformManagement";
    public const string BillingPolicy = "PlatformBilling";
    public const string AuditViewPolicy = "PlatformAuditView";
    public const string AuditExportPolicy = "PlatformAuditExport";
    public const string AuditSensitivePolicy = "PlatformAuditSensitive";
}
