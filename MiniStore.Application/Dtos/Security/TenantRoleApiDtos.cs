using System.ComponentModel.DataAnnotations;

namespace MiniStore.Application.DTOs.Security;

public sealed record TenantPermissionOptionDto(
    int Id,
    string Code,
    string GroupCode);

public sealed record TenantRoleFormDto(
    int Id,
    string Name,
    string? Description,
    bool IsSystem,
    IReadOnlyList<int> SelectedPermissionIds,
    IReadOnlyList<TenantPermissionOptionDto> Permissions,
    byte[] RowVersion);

public sealed class SaveTenantRoleDto
{
    public int Id { get; set; }

    [Required, StringLength(80)]
    public string Name { get; set; } = string.Empty;

    [StringLength(300)]
    public string? Description { get; set; }

    public List<int> SelectedPermissionIds { get; set; } = [];

    public byte[] RowVersion { get; set; } = [];
}
