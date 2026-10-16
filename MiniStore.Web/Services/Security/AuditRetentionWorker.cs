using Microsoft.Extensions.Options;
using MiniStore.Application.Services;
using MiniStore.Infrastructure.Services.Security;

namespace MiniStore.Web.Services;

public sealed class AuditRetentionWorker(
    IServiceScopeFactory scopeFactory,
    IOptions<AuditRetentionOptions> options,
    ILogger<AuditRetentionWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!options.Value.Enabled) return;
        var interval = TimeSpan.FromHours(Math.Clamp(options.Value.WorkerIntervalHours, 1, 168));
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var result = await scope.ServiceProvider.GetRequiredService<AuditRetentionService>()
                    .RunRetentionAsync(stoppingToken);
                logger.LogInformation("Audit retention completed: {DeletedCount} events removed after checkpoint {CheckpointId}",
                    result.DeletedCount, result.CheckpointId);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { break; }
            catch (Exception exception)
            {
                logger.LogCritical(exception, "Audit retention/checkpoint execution failed; no unarchived candidates should be deleted");
            }

            try { await Task.Delay(interval, stoppingToken); }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { break; }
        }
    }
}
