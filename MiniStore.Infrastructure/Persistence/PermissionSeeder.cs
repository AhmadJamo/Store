using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MiniStore.Application.Permissions;
using MiniStore.Domain.Entities;

namespace MiniStore.Infrastructure.Persistence;

public static class PermissionSeeder
{
    public static readonly (string Name, string Description, bool IsSystem)[] DefaultTenantRoles =
    [
        ("Admin", "Protected company owner and administration role.", true),
        ("WarehouseManager", "Warehouse and inventory operations.", false),
        ("Sales", "Sales and point-of-sale operations.", false),
        ("Accountant", "Accounting and purchase posting operations.", false)
    ];

    public static async Task SeedAsync(IServiceProvider services)
    {
        var context = services.GetRequiredService<AppDbContext>();
        var existingPermissionNames = (await context.Permissions.Select(x => x.Name).ToListAsync())
            .ToHashSet(StringComparer.Ordinal);
        foreach (var definition in PermissionDefinitions.All)
        {
            if (existingPermissionNames.Add(definition.Name))
                context.Permissions.Add(new Permission(
                    definition.Name, definition.DisplayName, definition.Group));
        }
        await context.SaveChangesAsync();

        var tenantIds = await context.Tenants.Select(x => x.Id).ToListAsync();
        var existingRoles = (await context.TenantRoles
                .Select(x => new { x.TenantId, x.NormalizedName }).ToListAsync())
            .Select(x => (x.TenantId, x.NormalizedName)).ToHashSet();
        foreach (var tenantId in tenantIds)
        {
            foreach (var definition in DefaultTenantRoles)
            {
                var normalized = definition.Name.ToUpperInvariant();
                if (existingRoles.Add((tenantId, normalized)))
                    await context.TenantRoles.AddAsync(new TenantRole(
                        tenantId, definition.Name, definition.Description, definition.IsSystem));
            }
        }
        await context.SaveChangesAsync();

        var permissions = await context.Permissions.ToListAsync();
        var adminRoles = await context.TenantRoles
            .Where(x => x.NormalizedName == "ADMIN").ToListAsync();
        var existingAssignments = (await context.TenantRolePermissions
                .Select(x => new { x.TenantRoleId, x.PermissionId }).ToListAsync())
            .Select(x => (x.TenantRoleId, x.PermissionId)).ToHashSet();
        foreach (var adminRole in adminRoles)
        {
            foreach (var permission in permissions)
            {
                if (existingAssignments.Add((adminRole.Id, permission.Id)))
                    context.TenantRolePermissions.Add(
                        new TenantRolePermission(adminRole.Id, permission.Id));
            }
        }
        await context.SaveChangesAsync();
    }
}
