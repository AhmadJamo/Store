using Microsoft.Extensions.Options;
using MiniStore.Application.Services;
using MiniStore.Infrastructure.Services.Security;

namespace MiniStore.Web.Services;

public sealed class AuditRetentionWorker(
    IServiceScopeFactory scopeFactory,
    IOptions<AuditRetentionOptions> options,
    IBackgroundJobMonitor monitor,
    ILogger<AuditRetentionWorker> logger) : BackgroundService
{
    private const string JobKey = "audit-retention";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var interval = TimeSpan.FromHours(Math.Clamp(options.Value.WorkerIntervalHours, 1, 168));
        monitor.Register(JobKey, "jobAuditRetention", "AuditEvents.View", interval, options.Value.Enabled);
        if (!options.Value.Enabled) { monitor.Disabled(JobKey); return; }
        while (!stoppingToken.IsCancellationRequested)
        {
            monitor.Started(JobKey);
            try
            {
                using var scope = scopeFactory.CreateScope();
                var result = await scope.ServiceProvider.GetRequiredService<AuditRetentionService>()
                    .RunRetentionAsync(stoppingToken);
                logger.LogInformation("Audit retention completed: {DeletedCount} events removed after checkpoint {CheckpointId}",
                    result.DeletedCount, result.CheckpointId);
                monitor.Succeeded(JobKey);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { break; }
            catch (Exception exception)
            {
                monitor.Failed(JobKey);
                logger.LogCritical(exception, "Audit retention/checkpoint execution failed; no unarchived candidates should be deleted");
            }

            try { await Task.Delay(interval, stoppingToken); }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { break; }
        }
    }
}
