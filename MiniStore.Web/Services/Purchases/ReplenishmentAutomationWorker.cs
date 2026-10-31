using MiniStore.Application.Services;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Web.Services;

public sealed class ReplenishmentAutomationWorker(
    IServiceScopeFactory scopeFactory,
    HttpTenantContext tenantContext,
    IBackgroundJobMonitor monitor,
    ILogger<ReplenishmentAutomationWorker> logger) : BackgroundService
{
    private const string JobKey = "replenishment-automation";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        monitor.Register(JobKey, "jobReplenishment", "PurchaseReplenishment.View", TimeSpan.FromMinutes(1));
        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(1));
        while (!stoppingToken.IsCancellationRequested)
        {
            monitor.Started(JobKey);
            try { await RunDueTenantsAsync(stoppingToken); monitor.Succeeded(JobKey); }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { break; }
            catch (Exception exception) { monitor.Failed(JobKey); logger.LogError(exception, "Scheduled replenishment scan failed"); }
            try { if (!await timer.WaitForNextTickAsync(stoppingToken)) break; }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { break; }
        }
    }

    private async Task RunDueTenantsAsync(CancellationToken cancellationToken)
    {
        List<int> tenantIds;
        using (var discoveryScope = scopeFactory.CreateScope())
            tenantIds = await discoveryScope.ServiceProvider.GetRequiredService<IReplenishmentAutomationRepository>()
                .GetDueTenantIdsAsync(DateTime.UtcNow);

        foreach (var tenantId in tenantIds)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using var tenant = tenantContext.BeginScope(tenantId);
            using var scope = scopeFactory.CreateScope();
            try { await scope.ServiceProvider.GetRequiredService<ReplenishmentAutomationService>().RunDueAsync(DateTime.UtcNow); }
            catch (Exception exception) { logger.LogError(exception, "Scheduled replenishment failed for tenant {TenantId}", tenantId); }
        }
    }
}
