using MiniStore.Domain.Entities;

namespace MiniStore.Application.DTOs.Security;

public class TenantRoleListItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsSystem { get; set; }
    public int UserCount { get; set; }
    public int PermissionCount { get; set; }
}

public class TenantRoleEditDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsSystem { get; set; }
    public List<int> SelectedPermissionIds { get; set; } = [];
    public List<Permission> Permissions { get; set; } = [];
    public byte[] RowVersion { get; set; } = [];
}

public record TenantRoleOptionDto(int Id, string Name, bool IsSystem);
