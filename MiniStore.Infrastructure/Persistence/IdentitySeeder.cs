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

        // ==========================================
        // 4. Make sure Admin has Admin role
        // ==========================================

        if (!await userManager.IsInRoleAsync(
                adminUser,
                "Admin"))
        {
            var roleResult = await userManager.AddToRoleAsync(
                adminUser,
                "Admin");
            if (!roleResult.Succeeded)
                throw new InvalidOperationException("Failed to assign the bootstrap administrator role.");
        }
    }
}
