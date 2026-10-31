using System.ComponentModel.DataAnnotations;

namespace MiniStore.Application.DTOs.Security;

public sealed record TenantUserListItemDto(
    string Id,
    string? UserName,
    string? Email,
    bool EmailConfirmed,
    IReadOnlyList<string> RoleNames);

public sealed record TenantUserCreatePageDto(IReadOnlyList<TenantRoleOptionDto> Roles);

public sealed class CreateTenantUserDto
{
    [Required, StringLength(100)]
    public string UserName { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required, StringLength(1024)]
    public string Password { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int RoleId { get; set; }
}

public interface ITenantUserAdministrationService
{
    Task<IReadOnlyList<TenantUserListItemDto>> GetAllAsync();
    Task<TenantUserCreatePageDto> GetCreateAsync();
    Task<TenantUserListItemDto> CreateAsync(CreateTenantUserDto dto);
}

public sealed class TenantUserCreationException(
    string message,
    IReadOnlyList<string>? identityErrorCodes = null) : InvalidOperationException(message)
{
    public IReadOnlyList<string> IdentityErrorCodes { get; } = identityErrorCodes ?? [];
}
