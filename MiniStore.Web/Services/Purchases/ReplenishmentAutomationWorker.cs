using MiniStore.Application.Services;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Web.Services;

public sealed class ReplenishmentAutomationWorker(
    IServiceScopeFactory scopeFactory,
    HttpTenantContext tenantContext,
    ILogger<ReplenishmentAutomationWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(1));
        while (!stoppingToken.IsCancellationRequested)
        {
            try { await RunDueTenantsAsync(stoppingToken); }
            catch (Exception exception) { logger.LogError(exception, "Scheduled replenishment scan failed"); }
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
