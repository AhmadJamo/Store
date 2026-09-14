using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;

namespace MiniStore.Infrastructure.Repositories;

public class TenantAuthorizationRepository(AppDbContext context) : ITenantAuthorizationRepository
{
    public Task<bool> HasRoleAsync(int tenantId, string userId, string roleName) =>
        context.TenantUserRoles.Join(context.Roles, x => x.RoleId, x => x.Id, (assignment, role) => new { assignment, role })
            .AnyAsync(x => x.assignment.TenantId == tenantId && x.assignment.UserId == userId && x.role.Name == roleName);
    public Task<bool> HasPermissionAsync(int tenantId, string userId, string permission) =>
        context.TenantUserRoles.Where(x => x.TenantId == tenantId && x.UserId == userId)
            .Join(context.RolePermissions, x => x.RoleId, x => x.RoleId, (_, rp) => rp)
            .Join(context.Permissions, x => x.PermissionId, x => x.Id, (_, p) => p)
            .AnyAsync(x => x.Name == permission);
    public Task AddRoleAsync(TenantUserRole assignment) => context.TenantUserRoles.AddAsync(assignment).AsTask();
    public Task SaveChangesAsync() => context.SaveChangesAsync();
}
