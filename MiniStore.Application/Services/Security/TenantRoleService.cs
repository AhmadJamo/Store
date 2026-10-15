using System.Security.Cryptography;
using System.Text;
using MiniStore.Application.DTOs.Security;
using MiniStore.Application.Permissions;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public class TenantRoleService(
    ITenantRoleRepository roles,
    ITenantAuthorizationRepository authorization,
    ITenantContext tenantContext,
    IUnitOfWork unitOfWork,
    SemanticAuditEventService audit)
{
    public async Task<List<TenantRoleListItemDto>> GetAllAsync()
    {
        var tenantId = RequireTenant();
        var result = new List<TenantRoleListItemDto>();
        foreach (var role in await roles.GetAllAsync(tenantId))
        {
            result.Add(new TenantRoleListItemDto
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
                IsSystem = role.IsSystem,
                UserCount = await roles.GetUserCountAsync(tenantId, role.Id),
                PermissionCount = await roles.GetPermissionCountAsync(role.Id)
            });
        }
        return result;
    }

    public async Task<TenantRoleEditDto> GetCreateAsync() => new()
    {
        Permissions = await GetDelegablePermissionsAsync()
    };

    public async Task<TenantRoleEditDto?> GetEditAsync(int roleId)
    {
        var role = await roles.GetAsync(RequireTenant(), roleId);
        if (role is null) return null;
        return new TenantRoleEditDto
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.Description,
            IsSystem = role.IsSystem,
            RowVersion = role.RowVersion,
            SelectedPermissionIds = (await roles.GetPermissionIdsAsync(role.Id)).ToList(),
            Permissions = await GetDelegablePermissionsAsync()
        };
    }

    public async Task CreateAsync(TenantRoleEditDto dto)
    {
        var tenantId = RequireTenant();
        var role = new TenantRole(tenantId, dto.Name, dto.Description);
        if (await roles.GetByNormalizedNameAsync(tenantId, role.NormalizedName) is not null)
            throw new InvalidOperationException("A role with this name already exists in this company.");
        var permissions = await ValidatePermissionsAsync(dto.SelectedPermissionIds);
        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            await roles.AddAsync(role);
            await unitOfWork.FlushAsync();
            await roles.ReplacePermissionsAsync(role.Id, permissions.Ids);
            await audit.RecordTenantSuccessAsync(
                AuditEventCategory.Security,
                SemanticAuditEventTypes.TenantRoleCreated,
                "Create",
                "TenantRole",
                role.Id.ToString(),
                afterValues: RoleAuditValues(role.Name, permissions.Codes));
        });
    }

    public async Task UpdateAsync(TenantRoleEditDto dto)
    {
        var tenantId = RequireTenant();
        var role = await roles.GetAsync(tenantId, dto.Id)
            ?? throw new InvalidOperationException("Role was not found in this company.");
        if (role.IsSystem) throw new InvalidOperationException("System roles cannot be edited.");
        if (dto.RowVersion.Length == 0 || !role.RowVersion.SequenceEqual(dto.RowVersion))
            throw new InvalidOperationException("Role settings changed in another session. Reload and try again.");
        var normalizedName = (dto.Name?.Trim() ?? string.Empty).ToUpperInvariant();
        var duplicate = await roles.GetByNormalizedNameAsync(tenantId, normalizedName);
        if (duplicate is not null && duplicate.Id != role.Id)
            throw new InvalidOperationException("A role with this name already exists in this company.");
        var oldPermissionIds = await roles.GetPermissionIdsAsync(role.Id);
        var oldPermissions = await ResolvePermissionCodesAsync(oldPermissionIds);
        var permissions = await ValidatePermissionsAsync(dto.SelectedPermissionIds);
        var before = RoleAuditValues(role.Name, oldPermissions);
        var permissionsChanged = !oldPermissionIds.Order().SequenceEqual(permissions.Ids.Order());
        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            role.Rename(dto.Name ?? string.Empty, dto.Description);
            await roles.ReplacePermissionsAsync(role.Id, permissions.Ids);
            await audit.RecordTenantSuccessAsync(
                AuditEventCategory.Security,
                permissionsChanged
                    ? SemanticAuditEventTypes.TenantRolePermissionsChanged
                    : SemanticAuditEventTypes.TenantRoleUpdated,
                "Update",
                "TenantRole",
                role.Id.ToString(),
                before,
                RoleAuditValues(role.Name, permissions.Codes));
        });
    }

    public async Task DeleteAsync(int roleId)
    {
        var tenantId = RequireTenant();
        var role = await roles.GetAsync(tenantId, roleId)
            ?? throw new InvalidOperationException("Role was not found in this company.");
        if (role.IsSystem) throw new InvalidOperationException("System roles cannot be deleted.");
        if (await roles.GetUserCountAsync(tenantId, role.Id) > 0)
            throw new InvalidOperationException("Remove all users from this role before deleting it.");
        var permissionCodes = await ResolvePermissionCodesAsync(await roles.GetPermissionIdsAsync(role.Id));
        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            roles.Remove(role);
            await audit.RecordTenantSuccessAsync(
                AuditEventCategory.Security,
                SemanticAuditEventTypes.TenantRoleDeleted,
                "Delete",
                "TenantRole",
                role.Id.ToString(),
                beforeValues: RoleAuditValues(role.Name, permissionCodes));
        });
    }

    public async Task<List<TenantRoleOptionDto>> GetOptionsAsync() =>
        (await roles.GetAllAsync(RequireTenant()))
            .Select(x => new TenantRoleOptionDto(x.Id, x.Name, x.IsSystem)).ToList();

    public Task<IReadOnlyDictionary<string, IReadOnlyList<string>>> GetUserRoleNamesAsync(
        IReadOnlyCollection<string> userIds) => roles.GetUserRoleNamesAsync(RequireTenant(), userIds);

    public async Task AssignUserAsync(string userId, int roleId)
    {
        var tenantId = RequireTenant();
        if (await roles.GetAsync(tenantId, roleId) is null)
            throw new InvalidOperationException("Selected role does not belong to this company.");
        await authorization.AddRoleAsync(new TenantUserRole(tenantId, userId, roleId));
    }

    private async Task<ValidatedPermissions> ValidatePermissionsAsync(IEnumerable<int>? selected)
    {
        var ids = (selected ?? []).Where(x => x > 0).Distinct().ToArray();
        var valid = (await GetDelegablePermissionsAsync()).ToDictionary(x => x.Id);
        if (ids.Any(x => !valid.ContainsKey(x)))
            throw new ArgumentException("One or more selected permissions are invalid.");
        return new ValidatedPermissions(ids, ids.Select(id => valid[id].Name).Order().ToArray());
    }

    private async Task<IReadOnlyCollection<string>> ResolvePermissionCodesAsync(IEnumerable<int> ids)
    {
        var requested = ids.ToHashSet();
        return (await roles.GetPermissionCatalogAsync())
            .Where(permission => requested.Contains(permission.Id))
            .Select(permission => permission.Name)
            .Order()
            .ToArray();
    }

    private static IReadOnlyDictionary<string, object?> RoleAuditValues(
        string name,
        IReadOnlyCollection<string> permissionCodes) => new Dictionary<string, object?>
    {
        ["RoleName"] = name,
        ["PermissionSetFingerprint"] = Convert.ToHexString(SHA256.HashData(
            Encoding.UTF8.GetBytes(string.Join('\n', permissionCodes.Order(StringComparer.Ordinal))))),
        ["PermissionCount"] = permissionCodes.Count
    };

    private sealed record ValidatedPermissions(
        IReadOnlyCollection<int> Ids,
        IReadOnlyCollection<string> Codes);

    private int RequireTenant() => tenantContext.TenantId
        ?? throw new InvalidOperationException("An active company is required.");

    private async Task<List<Permission>> GetDelegablePermissionsAsync() =>
        (await roles.GetPermissionCatalogAsync())
            .Where(x => !AdministrationPermissions.RequiresAdmin(x.Name))
            .ToList();
}
