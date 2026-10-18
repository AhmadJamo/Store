namespace MiniStore.Infrastructure.Services.Security;

public sealed class LegacyAuditOptions
{
    public const string SectionName = "LegacyAudit";

    public bool WriteEnabled { get; set; } = true;
}
