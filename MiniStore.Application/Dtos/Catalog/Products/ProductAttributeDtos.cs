using MiniStore.Domain.Entities;

namespace MiniStore.Application.DTOs.Products;

public sealed class CreateProductAttributeDto
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public ProductAttributeDataType DataType { get; set; } = ProductAttributeDataType.Text;
    public bool IsRequired { get; set; }
    public bool IsVariantDefining { get; set; }
    public int DisplayOrder { get; set; }
    public List<int> ProductCategoryIds { get; set; } = [];
    public string? OptionsText { get; set; }
}

public sealed class ProductAttributeDefinitionDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public ProductAttributeDataType DataType { get; set; }
    public bool IsRequired { get; set; }
    public bool IsVariantDefining { get; set; }
    public bool IsActive { get; set; }
    public int DisplayOrder { get; set; }
    public List<string> Categories { get; set; } = [];
    public List<string> Options { get; set; } = [];
}

public sealed class ProductAttributePageDto
{
    public CreateProductAttributeDto Form { get; set; } = new();
    public List<ProductAttributeDefinitionDto> Definitions { get; set; } = [];
    public List<ProductCategoryDto> Categories { get; set; } = [];
}

public sealed class ProductAttributeValueInputDto
{
    public int DefinitionId { get; set; }
    public string? Value { get; set; }
}

public sealed class ProductAttributeValueFieldDto
{
    public int DefinitionId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public ProductAttributeDataType DataType { get; set; }
    public bool IsRequired { get; set; }
    public bool IsVariantDefining { get; set; }
    public string? Value { get; set; }
    public List<ProductAttributeOptionFieldDto> Options { get; set; } = [];
}

public sealed class ProductAttributeOptionFieldDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}

public sealed class ProductAttributeValuesPageDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductCode { get; set; } = string.Empty;
    public string? CategoryName { get; set; }
    public List<ProductAttributeValueFieldDto> Fields { get; set; } = [];
    public List<ProductAttributeValueInputDto> Values { get; set; } = [];
}
