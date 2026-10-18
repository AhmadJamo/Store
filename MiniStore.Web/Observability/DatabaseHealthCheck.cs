using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MiniStore.Infrastructure.Persistence;

namespace MiniStore.Web.Observability;

public sealed class DatabaseHealthCheck(
    AppDbContext database,
    ILogger<DatabaseHealthCheck> logger) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        MiniStoreTelemetry.DatabaseHealthProbeCount.Add(1);

        try
        {
            if (await database.Database.CanConnectAsync(cancellationToken))
                return HealthCheckResult.Healthy();

            MiniStoreTelemetry.DatabaseHealthFailureCount.Add(1);
            logger.LogError("Database readiness probe could not establish a connection.");
            return HealthCheckResult.Unhealthy();
        }
        catch (Exception exception)
        {
            MiniStoreTelemetry.DatabaseHealthFailureCount.Add(1);
            logger.LogError(exception, "Database readiness probe failed.");
            return HealthCheckResult.Unhealthy();
        }
    }
}
