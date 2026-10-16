namespace MiniStore.Infrastructure.Services.Security;

public sealed class AuditDatabaseAccessOptions
{
    public required string ReadConnectionString { get; init; }
    public required string MaintenanceConnectionString { get; init; }
}
