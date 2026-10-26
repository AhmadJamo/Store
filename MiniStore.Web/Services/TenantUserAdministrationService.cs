using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MiniStore.Application.DTOs.Security;
using MiniStore.Application.Saas;
using MiniStore.Application.Services;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Web.Services;

public sealed class TenantUserAdministrationService(
    UserManager<IdentityUser> userManager,
    TenantRoleService roles,
    ITenantContext tenantContext,
    ITenantMembershipRepository memberships,
    EntitlementService entitlements,
    IUnitOfWork unitOfWork,
    SemanticAuditEventService audit) : ITenantUserAdministrationService
{
    public async Task<IReadOnlyList<TenantUserListItemDto>> GetAllAsync()
    {
        var tenantId = tenantContext.TenantId
            ?? throw new InvalidOperationException("An active company is required.");
        var tenantUserIds = await memberships.GetActiveUserIdsAsync(tenantId);
        var users = await userManager.Users
            .Where(user => tenantUserIds.Contains(user.Id))
            .OrderBy(user => user.UserName)
            .ToListAsync();
        var roleNames = await roles.GetUserRoleNamesAsync(users.Select(user => user.Id).ToArray());
        return users.Select(user => new TenantUserListItemDto(
            user.Id,
            user.UserName,
            user.Email,
            user.EmailConfirmed,
            roleNames.GetValueOrDefault(user.Id) ?? [])).ToList();
    }

    public async Task<TenantUserCreatePageDto> GetCreateAsync() =>
        new(await roles.GetOptionsAsync());

    public async Task<TenantUserListItemDto> CreateAsync(CreateTenantUserDto dto)
    {
        await entitlements.EnsureLimitAsync(SaasLimitKeys.Users);
        var userName = dto.UserName.Trim();
        var email = dto.Email.Trim();
        if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(dto.Password) || dto.RoleId <= 0)
            throw new ArgumentException("All fields are required.");
        if (await userManager.FindByNameAsync(userName) is not null)
            throw new TenantUserCreationException("Username already exists.");

        var selectedRole = (await roles.GetOptionsAsync()).SingleOrDefault(role => role.Id == dto.RoleId)
            ?? throw new ArgumentException("Selected role does not exist.");
        var user = new IdentityUser { UserName = userName, Email = email, EmailConfirmed = true };
        var result = await userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            throw new TenantUserCreationException(
                "The user account could not be created.",
                result.Errors.Select(error => error.Code).Distinct().ToArray());

        try
        {
            var tenantId = tenantContext.TenantId
                ?? throw new InvalidOperationException("An active company is required.");
            await unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                await memberships.AddAsync(new TenantMembership(tenantId, user.Id));
                await roles.AssignUserAsync(user.Id, selectedRole.Id);
                await audit.RecordTenantSuccessAsync(
                    AuditEventCategory.Security,
                    SemanticAuditEventTypes.TenantUserCreated,
                    "Create",
                    "TenantUser",
                    user.Id);
            });
        }
        catch
        {
            await userManager.DeleteAsync(user);
            throw;
        }

        return new TenantUserListItemDto(
            user.Id, user.UserName, user.Email, user.EmailConfirmed, [selectedRole.Name]);
    }
}
