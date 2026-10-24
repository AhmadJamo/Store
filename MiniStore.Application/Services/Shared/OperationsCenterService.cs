using MiniStore.Application.Dtos.Shared;
using MiniStore.Application.Permissions;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public sealed class OperationsCenterService(
    IBackgroundJobMonitor monitor,
    IAnalyticsWorkspaceRepository workspaces,
    IPermissionService permissions,
    ICurrentUserService currentUser)
{
    public async Task<OperationsCenterDto> GetAsync()
    {
        var granted = (await permissions.GetGrantedAsync(currentUser.UserId))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var jobs = monitor.Snapshot(DateTime.UtcNow)
            .Where(x => granted.Contains(x.Permission))
            .OrderBy(x => x.TitleKey)
            .ToList();
        var plans = new List<AnalyticsRefreshPlanDto>();
        if (granted.Contains("AnalyticsCenter.View"))
        {
            plans = (await workspaces.GetAvailableAsync(currentUser.UserId))
                .Where(x => x.RefreshIntervalMinutes > 0)
                .OrderBy(x => x.Name)
                .Select(x => new AnalyticsRefreshPlanDto(
                    x.Id, x.Name, x.NameArabic, x.ModuleKey, x.RefreshIntervalMinutes,
                    IsAutomaticDelivery: false))
                .ToList();
        }

        return new OperationsCenterDto(DateTime.UtcNow, jobs, plans);
    }
}
