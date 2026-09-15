using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MiniStore.Application.Saas;
using MiniStore.Domain.Entities;
using MiniStore.Infrastructure.Persistence;

namespace MiniStore.Infrastructure.Services;

public class SaasOnboardingService(
    AppDbContext context,
    UserManager<IdentityUser> users) : ISaasOnboardingService
{
    public async Task<RegisterCompanyResult> RegisterAsync(RegisterCompanyCommand command)
    {
        var username = command.OwnerUsername?.Trim() ?? string.Empty;
        var email = command.OwnerEmail?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Owner username and email are required.");

        if (await users.FindByNameAsync(username) is not null)
            throw new InvalidOperationException("This username is already registered.");
        if (await users.FindByEmailAsync(email) is not null)
            throw new InvalidOperationException("This email is already registered.");
        if (await context.Tenants.AnyAsync(x => x.Slug == command.CompanySlug.Trim().ToLowerInvariant()))
            throw new InvalidOperationException("This company address is already used.");

        var plan = await context.Plans.FirstOrDefaultAsync(x =>
            x.Id == command.PlanId && x.IsActive && x.IsPublic)
            ?? throw new InvalidOperationException("The selected plan is unavailable.");

        await using var transaction = await context.Database.BeginTransactionAsync();
        var user = new IdentityUser
        {
            UserName = username,
            Email = email,
            EmailConfirmed = false
        };
        var createUser = await users.CreateAsync(user, command.Password);
        if (!createUser.Succeeded)
            throw new InvalidOperationException(string.Join(" ", createUser.Errors.Select(x => x.Description)));

        var tenant = new Tenant(command.CompanyName, command.CompanySlug);
        await context.Tenants.AddAsync(tenant);
        await context.SaveChangesAsync();

        await context.TenantMemberships.AddAsync(new TenantMembership(tenant.Id, user.Id, isOwner: true));
        await context.CompanyOnboardings.AddAsync(new CompanyOnboarding(tenant.Id));
        await context.TenantSubscriptions.AddAsync(
            new TenantSubscription(tenant.Id, plan.Id, DateTime.UtcNow.AddDays(14)));
        foreach (var definition in PermissionSeeder.DefaultTenantRoles)
            await context.TenantRoles.AddAsync(
                new TenantRole(tenant.Id, definition.Name, definition.Description, definition.IsSystem));
        await context.SaveChangesAsync();

        var adminRole = await context.TenantRoles.SingleAsync(x =>
            x.TenantId == tenant.Id && x.NormalizedName == "ADMIN");
        var permissionIds = await context.Permissions.Select(x => x.Id).ToListAsync();
        await context.TenantRolePermissions.AddRangeAsync(
            permissionIds.Select(permissionId => new TenantRolePermission(adminRole.Id, permissionId)));
        await context.TenantUserRoles.AddAsync(new TenantUserRole(tenant.Id, user.Id, adminRole.Id));

        await context.SaveChangesAsync();
        await transaction.CommitAsync();
        return new RegisterCompanyResult(user.Id, tenant.Id, tenant.Name);
    }
}
