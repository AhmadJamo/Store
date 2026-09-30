namespace MiniStore.Domain.Entities;

public sealed class ProductAttributeValue
{
    private ProductAttributeValue() { }

    public int Id { get; private set; }
    public int ProductId { get; private set; }
    public int ProductAttributeDefinitionId { get; private set; }
    public string? TextValue { get; private set; }
    public decimal? NumberValue { get; private set; }
    public bool? BooleanValue { get; private set; }
    public int? ProductAttributeOptionId { get; private set; }

    public ProductAttributeValue(
        int productId,
        int definitionId,
        string? textValue,
        decimal? numberValue,
        bool? booleanValue,
        int? optionId)
    {
        if (productId <= 0 || definitionId <= 0)
            throw new ArgumentException("Product and attribute definition are required.");
        var valueCount = (string.IsNullOrWhiteSpace(textValue) ? 0 : 1) +
                         (numberValue.HasValue ? 1 : 0) +
                         (booleanValue.HasValue ? 1 : 0) +
                         (optionId.HasValue ? 1 : 0);
        if (valueCount != 1)
            throw new ArgumentException("An attribute value must contain exactly one typed value.");
        if (textValue?.Trim().Length > 500)
            throw new ArgumentException("Text attribute values cannot exceed 500 characters.");

        ProductId = productId;
        ProductAttributeDefinitionId = definitionId;
        TextValue = string.IsNullOrWhiteSpace(textValue) ? null : textValue.Trim();
        NumberValue = numberValue;
        BooleanValue = booleanValue;
        ProductAttributeOptionId = optionId;
    }
}
