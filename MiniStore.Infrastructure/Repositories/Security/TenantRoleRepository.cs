using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;

namespace MiniStore.Infrastructure.Repositories;

public class TenantRoleRepository(AppDbContext context) : ITenantRoleRepository
{
    public async Task<IReadOnlyList<TenantRole>> GetAllAsync(int tenantId) =>
        await context.TenantRoles.Where(x => x.TenantId == tenantId).OrderBy(x => x.Name).ToListAsync();

    public Task<TenantRole?> GetAsync(int tenantId, int roleId) =>
        context.TenantRoles.SingleOrDefaultAsync(x => x.TenantId == tenantId && x.Id == roleId);

    public Task<TenantRole?> GetByNormalizedNameAsync(int tenantId, string normalizedName) =>
        context.TenantRoles.SingleOrDefaultAsync(x =>
            x.TenantId == tenantId && x.NormalizedName == normalizedName);

    public async Task<IReadOnlyList<Permission>> GetPermissionCatalogAsync() =>
        await context.Permissions.OrderBy(x => x.Group).ThenBy(x => x.Name).ToListAsync();

    public async Task<IReadOnlyList<int>> GetPermissionIdsAsync(int roleId) =>
        await context.TenantRolePermissions.Where(x => x.TenantRoleId == roleId)
            .Select(x => x.PermissionId).ToListAsync();

    public Task<int> GetUserCountAsync(int tenantId, int roleId) =>
        context.TenantUserRoles.CountAsync(x => x.TenantId == tenantId && x.TenantRoleId == roleId);

    public Task<int> GetPermissionCountAsync(int roleId) =>
        context.TenantRolePermissions.CountAsync(x => x.TenantRoleId == roleId);

    public async Task<IReadOnlyDictionary<string, IReadOnlyList<string>>> GetUserRoleNamesAsync(
        int tenantId,
        IReadOnlyCollection<string> userIds)
    {
        var rows = await context.TenantUserRoles
            .Where(x => x.TenantId == tenantId && userIds.Contains(x.UserId))
            .Join(context.TenantRoles.Where(x => x.TenantId == tenantId),
                assignment => assignment.TenantRoleId, role => role.Id,
                (assignment, role) => new { assignment.UserId, role.Name })
            .OrderBy(x => x.Name)
            .ToListAsync();
        return rows.GroupBy(x => x.UserId)
            .ToDictionary(x => x.Key, x => (IReadOnlyList<string>)x.Select(y => y.Name).ToList());
    }

    public async Task CreateAsync(TenantRole role, IReadOnlyCollection<int> permissionIds)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            await context.TenantRoles.AddAsync(role);
            await context.SaveChangesAsync();
            await context.TenantRolePermissions.AddRangeAsync(
                permissionIds.Select(permissionId => new TenantRolePermission(role.Id, permissionId)));
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (DbUpdateException exception)
        {
            await transaction.RollbackAsync();
            throw new InvalidOperationException(
                "The role could not be created because its name or permissions conflict.", exception);
        }
    }

    public async Task ReplacePermissionsAsync(int roleId, IReadOnlyCollection<int> permissionIds)
    {
        var current = await context.TenantRolePermissions.Where(x => x.TenantRoleId == roleId).ToListAsync();
        var requested = permissionIds.ToHashSet();
        context.TenantRolePermissions.RemoveRange(current.Where(x => !requested.Contains(x.PermissionId)));
        var currentIds = current.Select(x => x.PermissionId).ToHashSet();
        await context.TenantRolePermissions.AddRangeAsync(
            requested.Where(permissionId => !currentIds.Contains(permissionId))
                .Select(permissionId => new TenantRolePermission(roleId, permissionId)));
    }

    public void Remove(TenantRole role) => context.TenantRoles.Remove(role);

    public async Task SaveChangesAsync()
    {
        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException exception)
        {
            throw new InvalidOperationException("Role settings changed in another session. Reload and try again.", exception);
        }
        catch (DbUpdateException exception)
        {
            throw new InvalidOperationException("The role could not be saved because its name or assignments conflict.", exception);
        }
    }
}
