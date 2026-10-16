namespace MiniStore.Infrastructure.Services.Security;

public sealed class AuditRetentionOptions
{
    public const string SectionName = "AuditRetention";
    public bool Enabled { get; set; }
    public string ArchiveDirectory { get; set; } = string.Empty;
    public string CheckpointKey { get; set; } = string.Empty;
    public int ProtectedRetentionYears { get; set; } = 7;
    public int OperationalRetentionYears { get; set; } = 3;
    public int RequestEvidenceDays { get; set; } = 90;
    public int BatchSize { get; set; } = 1000;
    public int WorkerIntervalHours { get; set; } = 24;
}
