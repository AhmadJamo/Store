namespace MiniStore.Domain.Entities;

public class Tenant
{
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;
    public bool IsActive { get; private set; } = true;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public byte[] RowVersion { get; private set; } = Array.Empty<byte>();

    private Tenant()
    {
    }

    public Tenant(string name, string slug)
    {
        Rename(name);
        ChangeSlug(slug);
    }

    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Trim().Length > 200)
            throw new ArgumentException("Tenant name is required and cannot exceed 200 characters.");
        Name = name.Trim();
    }

    public void ChangeSlug(string slug)
    {
        var value = slug?.Trim().ToLowerInvariant() ?? string.Empty;
        if (!System.Text.RegularExpressions.Regex.IsMatch(value, "^[a-z0-9][a-z0-9-]{1,61}[a-z0-9]$"))
            throw new ArgumentException("Tenant slug must contain 3-63 lowercase letters, numbers or hyphens.");
        Slug = value;
    }

    public void SetActive(bool isActive) => IsActive = isActive;
}
