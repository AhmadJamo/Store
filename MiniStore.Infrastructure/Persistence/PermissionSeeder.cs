using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MiniStore.Application.Permissions;
using MiniStore.Domain.Entities;

namespace MiniStore.Infrastructure.Persistence;

public static class PermissionSeeder
{
    public static async Task SeedAsync(
        IServiceProvider services)
    {
        var context =
            services.GetRequiredService<AppDbContext>();

        var roleManager =
            services.GetRequiredService<
                RoleManager<IdentityRole>>();

        // 1. Create permissions
        foreach (var definition in PermissionDefinitions.All)
        {
            var exists =
                await context.Permissions
                    .AnyAsync(x => x.Name == definition.Name);

            if (!exists)
            {
                context.Permissions.Add(
                    new Permission(
                        definition.Name,
                        definition.DisplayName,
                        definition.Group));
            }
        }

        await context.SaveChangesAsync();

        // 2. Get Admin role
        var adminRole =
            await roleManager.FindByNameAsync("Admin");

        if (adminRole == null)
        {
            throw new InvalidOperationException(
                "Admin role does not exist.");
        }

        // 3. Get all permissions
        var permissions =
            await context.Permissions
                .ToListAsync();

        // 4. Give Admin all permissions
        foreach (var permission in permissions)
        {
            var exists =
                await context.RolePermissions
                    .AnyAsync(x =>
                        x.RoleId == adminRole.Id &&
                        x.PermissionId == permission.Id);

            if (!exists)
            {
                context.RolePermissions.Add(
                    new RolePermission(
                        adminRole.Id,
                        permission.Id));
            }
        }

        await context.SaveChangesAsync();
    }
}