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
        await context.TenantSubscriptions.AddAsync(
            new TenantSubscription(tenant.Id, plan.Id, DateTime.UtcNow.AddDays(14)));
        var roleResult = await users.AddToRoleAsync(user, "Admin");
        if (!roleResult.Succeeded)
            throw new InvalidOperationException("The company owner role could not be assigned.");

        var adminRoleId = await context.Roles.Where(x => x.Name == "Admin").Select(x => x.Id).FirstAsync();
        await context.TenantUserRoles.AddAsync(new TenantUserRole(tenant.Id, user.Id, adminRoleId));

        await context.SaveChangesAsync();
        await transaction.CommitAsync();
        return new RegisterCompanyResult(user.Id, tenant.Id, tenant.Name);
    }
}
