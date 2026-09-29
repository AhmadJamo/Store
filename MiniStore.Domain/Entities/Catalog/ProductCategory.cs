namespace MiniStore.Domain.Entities;

public sealed class ProductCategory
{
    private ProductCategory()
    {
        Name = string.Empty;
        Code = string.Empty;
    }

    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Code { get; private set; } = string.Empty;
    public bool IsActive { get; private set; } = true;
    public byte[] RowVersion { get; private set; } = [];

    public ProductCategory(string name, string code)
    {
        Rename(name);
        ChangeCode(code);
    }

    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Trim().Length > 100)
            throw new ArgumentException("Category name is required and cannot exceed 100 characters.");
        Name = name.Trim();
    }

    public void ChangeCode(string code)
    {
        var value = code?.Trim().ToUpperInvariant() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(value) || value.Length > 30 ||
            !System.Text.RegularExpressions.Regex.IsMatch(value, "^[A-Z0-9][A-Z0-9_-]*$"))
        {
            throw new ArgumentException("Category code must use letters, numbers, hyphens or underscores and cannot exceed 30 characters.");
        }
        Code = value;
    }

    public void SetActive(bool isActive) => IsActive = isActive;
}
