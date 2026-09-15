namespace MiniStore.Domain.Entities;

public class TenantRole
{
    public int Id { get; private set; }
    public int TenantId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string NormalizedName { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public bool IsSystem { get; private set; }
    public byte[] RowVersion { get; private set; } = [];

    private TenantRole() { }

    public TenantRole(int tenantId, string name, string? description = null, bool isSystem = false)
    {
        if (tenantId <= 0) throw new ArgumentException("A valid company is required.", nameof(tenantId));
        TenantId = tenantId;
        ApplyDetails(name, description);
        IsSystem = isSystem;
    }

    public void Rename(string name, string? description)
    {
        if (IsSystem) throw new InvalidOperationException("System roles cannot be renamed.");
        ApplyDetails(name, description);
    }

    private void ApplyDetails(string name, string? description)
    {
        name = name?.Trim() ?? string.Empty;
        description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        if (name.Length is < 2 or > 80 || name.Any(char.IsControl))
            throw new ArgumentException("Role name must be between 2 and 80 safe characters.", nameof(name));
        if (description?.Length > 300 || description?.Any(char.IsControl) == true)
            throw new ArgumentException("Role description must be at most 300 safe characters.", nameof(description));
        Name = name;
        NormalizedName = name.ToUpperInvariant();
        Description = description;
    }
}
