namespace MiniStore.Domain.Entities;

public enum ProductAttributeDataType
{
    Text = 1,
    Number = 2,
    Boolean = 3,
    Selection = 4
}

public sealed class ProductAttributeDefinition
{
    private ProductAttributeDefinition() { Name = Code = string.Empty; }

    public int Id { get; private set; }
    public string Name { get; private set; }
    public string Code { get; private set; }
    public ProductAttributeDataType DataType { get; private set; }
    public bool IsRequired { get; private set; }
    public bool IsVariantDefining { get; private set; }
    public bool IsActive { get; private set; } = true;
    public int DisplayOrder { get; private set; }
    public byte[] RowVersion { get; private set; } = [];

    public ProductAttributeDefinition(string name, string code, ProductAttributeDataType dataType,
        bool isRequired, bool isVariantDefining, int displayOrder)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Trim().Length > 100)
            throw new ArgumentException("Attribute name is required and cannot exceed 100 characters.");
        var normalizedCode = code?.Trim().ToUpperInvariant() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(normalizedCode) || normalizedCode.Length > 40 ||
            !System.Text.RegularExpressions.Regex.IsMatch(normalizedCode, "^[A-Z0-9][A-Z0-9_-]*$"))
            throw new ArgumentException("Attribute code must use letters, numbers, hyphens or underscores and cannot exceed 40 characters.");
        if (!Enum.IsDefined(dataType))
            throw new ArgumentException("Attribute data type is invalid.");
        if (displayOrder < 0)
            throw new ArgumentException("Attribute display order cannot be negative.");

        Name = name.Trim();
        Code = normalizedCode;
        DataType = dataType;
        IsRequired = isRequired;
        IsVariantDefining = isVariantDefining;
        DisplayOrder = displayOrder;
    }

    public void SetActive(bool isActive) => IsActive = isActive;
}

public sealed class ProductAttributeOption
{
    private ProductAttributeOption() { Name = Code = string.Empty; }
    public int Id { get; private set; }
    public int ProductAttributeDefinitionId { get; private set; }
    public string Name { get; private set; }
    public string Code { get; private set; }
    public int DisplayOrder { get; private set; }
    public bool IsActive { get; private set; } = true;

    public ProductAttributeOption(int definitionId, string name, string code, int displayOrder)
    {
        if (definitionId <= 0) throw new ArgumentException("Attribute definition is required.");
        if (string.IsNullOrWhiteSpace(name) || name.Trim().Length > 100)
            throw new ArgumentException("Attribute option name is required and cannot exceed 100 characters.");
        var normalizedCode = code?.Trim().ToUpperInvariant() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(normalizedCode) || normalizedCode.Length > 40 ||
            !System.Text.RegularExpressions.Regex.IsMatch(normalizedCode, "^[A-Z0-9][A-Z0-9_-]*$"))
            throw new ArgumentException("Attribute option code is invalid.");
        if (displayOrder < 0) throw new ArgumentException("Attribute option order cannot be negative.");
        ProductAttributeDefinitionId = definitionId;
        Name = name.Trim();
        Code = normalizedCode;
        DisplayOrder = displayOrder;
    }
}

public sealed class ProductCategoryAttribute
{
    private ProductCategoryAttribute() { }
    public int ProductCategoryId { get; private set; }
    public int ProductAttributeDefinitionId { get; private set; }
    public ProductCategoryAttribute(int categoryId, int definitionId)
    {
        if (categoryId <= 0 || definitionId <= 0)
            throw new ArgumentException("Product category and attribute definition are required.");
        ProductCategoryId = categoryId;
        ProductAttributeDefinitionId = definitionId;
    }
}
