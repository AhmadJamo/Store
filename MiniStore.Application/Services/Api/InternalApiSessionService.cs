using MiniStore.Application.DTOs.Api;
using MiniStore.Application.Permissions;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services.Api;

public sealed class InternalApiSessionService(
    ITenantContext tenantContext,
    ISaasRepository tenants,
    IPermissionService permissions)
{
    public async Task<InternalApiSessionDto> GetAsync(
        string userId,
        string? userName,
        string? email,
        string culture,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (tenantContext.TenantId is not int tenantId)
            throw new UnauthorizedAccessException("An active company session is required.");

        var tenant = await tenants.GetTenantAsync(tenantId)
            ?? throw new UnauthorizedAccessException("The active company was not found.");
        cancellationToken.ThrowIfCancellationRequested();

        var granted = await permissions.GetGrantedAsync(userId);
        return new InternalApiSessionDto(
            new InternalApiUserDto(userId, userName, email),
            new InternalApiTenantDto(tenant.Id, tenant.Name, tenant.Slug),
            culture,
            culture.StartsWith("ar", StringComparison.OrdinalIgnoreCase) ? "rtl" : "ltr",
            granted);
    }
}
