namespace MiniStore.Infrastructure.Services.Security;

public sealed class AuditEventOptions
{
    public const string SectionName = "AuditEvents";

    public bool Enabled { get; set; }
    public string ActiveIntegrityKeyVersion { get; set; } = string.Empty;
    public Dictionary<string, string> IntegrityKeys { get; set; } = new(StringComparer.Ordinal);
    public int MaxChangesJsonLength { get; set; } = 16000;
    public int MaxMetadataJsonLength { get; set; } = 8000;
    public Dictionary<string, string[]> AllowedChangeFields { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, string[]> AllowedMetadataFields { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public string[] AllowedReasonTextEventTypes { get; set; } = [];
}
