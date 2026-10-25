using Microsoft.EntityFrameworkCore;
using MiniStore.Application.Services;
using MiniStore.Infrastructure.Persistence;

namespace MiniStore.Infrastructure.Repositories;

public sealed class AnalyticsPrincipalDirectory(AppDbContext context) : IAnalyticsPrincipalDirectory
{
    public async Task<IReadOnlyList<AnalyticsPrincipalOption>> GetUsersAsync(int tenantId) =>
        await context.TenantMemberships
            .Where(x => x.TenantId == tenantId && x.IsActive)
            .Join(context.Users,
                membership => membership.UserId,
                user => user.Id,
                (_, user) => new AnalyticsPrincipalOption(
                    user.Id,
                    user.UserName ?? user.Email ?? user.Id))
            .OrderBy(x => x.DisplayName)
            .ToListAsync();

    public async Task<IReadOnlyList<AnalyticsPrincipalOption>> GetRolesAsync(int tenantId) =>
        await context.TenantRoles
            .Where(x => x.TenantId == tenantId)
            .OrderBy(x => x.Name)
            .Select(x => new AnalyticsPrincipalOption(x.Id.ToString(), x.Name))
            .ToListAsync();
}
