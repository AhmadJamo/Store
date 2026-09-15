using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;

namespace MiniStore.Infrastructure.Repositories;

public class TenantAuthorizationRepository(AppDbContext context) : ITenantAuthorizationRepository
{
    public Task<bool> HasRoleAsync(int tenantId, string userId, string roleName)
    {
        var normalizedRoleName = roleName.Trim().ToUpperInvariant();
        return
        context.TenantUserRoles.Join(context.TenantRoles, x => x.TenantRoleId, x => x.Id, (assignment, role) => new { assignment, role })
            .AnyAsync(x => x.assignment.TenantId == tenantId && x.assignment.UserId == userId &&
                           x.role.TenantId == tenantId && x.role.NormalizedName == normalizedRoleName);
    }
    public Task<bool> HasPermissionAsync(int tenantId, string userId, string permission) =>
        context.TenantUserRoles.Where(x => x.TenantId == tenantId && x.UserId == userId)
            .Join(context.TenantRolePermissions, x => x.TenantRoleId, x => x.TenantRoleId, (_, rp) => rp)
            .Join(context.Permissions, x => x.PermissionId, x => x.Id, (_, p) => p)
            .AnyAsync(x => x.Name == permission);
    public async Task AddRoleAsync(TenantUserRole assignment)
    {
        var hasMembership = context.TenantMemberships.Local.Any(x =>
                x.TenantId == assignment.TenantId && x.UserId == assignment.UserId && x.IsActive) ||
            await context.TenantMemberships.AnyAsync(x =>
                x.TenantId == assignment.TenantId && x.UserId == assignment.UserId && x.IsActive);
        if (!hasMembership)
            throw new InvalidOperationException("A role can be assigned only to an active company member.");

        if (!await context.TenantRoles.AnyAsync(x =>
                x.Id == assignment.TenantRoleId && x.TenantId == assignment.TenantId))
            throw new InvalidOperationException("The selected role does not belong to this company.");

        var alreadyAssigned = context.TenantUserRoles.Local.Any(x =>
                x.TenantId == assignment.TenantId && x.UserId == assignment.UserId &&
                x.TenantRoleId == assignment.TenantRoleId) ||
            await context.TenantUserRoles.AnyAsync(x =>
                x.TenantId == assignment.TenantId && x.UserId == assignment.UserId &&
                x.TenantRoleId == assignment.TenantRoleId);
        if (!alreadyAssigned) await context.TenantUserRoles.AddAsync(assignment);
    }
    public Task SaveChangesAsync() => context.SaveChangesAsync();
}
