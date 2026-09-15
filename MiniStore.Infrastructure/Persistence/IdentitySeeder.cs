using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MiniStore.Infrastructure.Persistence;

public static class IdentitySeeder
{
    public static async Task SeedAsync(
        IServiceProvider services,
        IConfiguration configuration)
    {
        var roleManager =
            services.GetRequiredService<
                RoleManager<IdentityRole>>();

        var userManager =
            services.GetRequiredService<
                UserManager<IdentityUser>>();

        // ==========================================
        // 1. Create default roles
        // ==========================================

        string[] roles =
        {
            "Admin",
            "WarehouseManager",
            "Sales",
            "Accountant"
        };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                var result =
                    await roleManager.CreateAsync(
                        new IdentityRole(role));

                if (!result.Succeeded)
                {
                    var errors =
                        string.Join(
                            ", ",
                            result.Errors.Select(x =>
                                x.Description));

                    throw new InvalidOperationException(
                        $"Failed to create role '{role}': {errors}");
                }
            }
        }

        // ==========================================
        // 2. Admin configuration
        // ==========================================

        if (!configuration.GetValue<bool>("AdminUser:Enabled"))
            return;

        var adminUsername =
            configuration["AdminUser:Username"];

        var adminEmail =
            configuration["AdminUser:Email"];

        var adminPassword =
            configuration["AdminUser:Password"];

        if (string.IsNullOrWhiteSpace(adminUsername))
            throw new InvalidOperationException(
                "Admin username is not configured.");

        if (string.IsNullOrWhiteSpace(adminEmail))
            throw new InvalidOperationException(
                "Admin email is not configured.");

        if (string.IsNullOrWhiteSpace(adminPassword))
            throw new InvalidOperationException(
                "Admin password is not configured.");

        // ==========================================
        // 3. Create Admin user
        // ==========================================

        var adminUser =
            await userManager.FindByNameAsync(
                adminUsername);

        if (adminUser != null)
            throw new InvalidOperationException("Bootstrap requires a new username; existing accounts are never promoted.");

        if (adminUser == null)
        {
            adminUser = new IdentityUser
            {
                UserName = adminUsername,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var result =
                await userManager.CreateAsync(
                    adminUser,
                    adminPassword);

            if (!result.Succeeded)
            {
                var errors =
                    string.Join(
                        ", ",
                        result.Errors.Select(x =>
                            x.Description));

                throw new InvalidOperationException(
                    $"Failed to create admin user: {errors}");
            }
        }

        var context = services.GetRequiredService<AppDbContext>();
        var tenantId = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions
            .FirstOrDefaultAsync(
                context.Tenants
                    .Where(x => x.IsActive)
                    .OrderBy(x => x.Id)
                    .Select(x => (int?)x.Id));

        if (tenantId is int activeTenantId)
        {
            await context.TenantMemberships.AddAsync(
                new MiniStore.Domain.Entities.TenantMembership(
                    activeTenantId,
                    adminUser.Id,
                    isOwner: true));
            var adminRole = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(
                context.TenantRoles.Where(x => x.TenantId == activeTenantId && x.NormalizedName == "ADMIN"));
            if (adminRole is null)
            {
                adminRole = new MiniStore.Domain.Entities.TenantRole(
                    activeTenantId, "Admin", "Protected company owner and administration role.", true);
                await context.TenantRoles.AddAsync(adminRole);
                await context.SaveChangesAsync();
            }
            await context.TenantUserRoles.AddAsync(
                new MiniStore.Domain.Entities.TenantUserRole(activeTenantId, adminUser.Id, adminRole.Id));
            await context.SaveChangesAsync();
        }
    }
}
