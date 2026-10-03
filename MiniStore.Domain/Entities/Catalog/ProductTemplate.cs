namespace MiniStore.Domain.Entities;

public sealed class ProductTemplate
{
    private ProductTemplate() { Name = Code = string.Empty; }

    public int Id { get; private set; }
    public string Name { get; private set; }
    public string Code { get; private set; }
    public int ProductCategoryId { get; private set; }
    public bool IsActive { get; private set; } = true;
    public byte[] RowVersion { get; private set; } = [];

    public ProductTemplate(string name, string code, int productCategoryId)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Trim().Length > 150)
            throw new ArgumentException("Template name is required and cannot exceed 150 characters.");
        var normalizedCode = code?.Trim().ToUpperInvariant() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(normalizedCode) || normalizedCode.Length > 40 ||
            !System.Text.RegularExpressions.Regex.IsMatch(normalizedCode, "^[A-Z0-9][A-Z0-9_-]*$"))
            throw new ArgumentException("Template code must use letters, numbers, hyphens or underscores and cannot exceed 40 characters.");
        if (productCategoryId <= 0) throw new ArgumentException("Product template category is required.");
        Name = name.Trim();
        Code = normalizedCode;
        ProductCategoryId = productCategoryId;
    }

    public void SetActive(bool isActive) => IsActive = isActive;
}
