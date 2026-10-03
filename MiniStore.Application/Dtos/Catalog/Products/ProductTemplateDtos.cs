namespace MiniStore.Application.DTOs.Products;

public sealed class CreateProductTemplateDto
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public int ProductCategoryId { get; set; }
}

public sealed class AssignProductTemplateDto
{
    public int ProductId { get; set; }
    public int? ProductTemplateId { get; set; }
}

public sealed class ProductTemplateRowDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public List<ProductVariantRowDto> Variants { get; set; } = [];
    public List<VariantGenerationAttributeDto> GenerationAttributes { get; set; } = [];
}

public sealed class VariantGenerationAttributeDto
{
    public int DefinitionId { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<ProductAttributeOptionFieldDto> Options { get; set; } = [];
}

public sealed class VariantGenerationInputDto
{
    public int ProductTemplateId { get; set; }
    public int SourceProductId { get; set; }
    public List<int> OptionIds { get; set; } = [];
}

public sealed class VariantGenerationPreviewDto
{
    public int ProductTemplateId { get; set; }
    public int SourceProductId { get; set; }
    public List<int> OptionIds { get; set; } = [];
    public List<VariantGenerationPreviewRowDto> Rows { get; set; } = [];
    public int ExistingCount { get; set; }
    public int NewCount => Rows.Count - ExistingCount;
}

public sealed class VariantGenerationPreviewRowDto
{
    public string Label { get; set; } = string.Empty;
    public bool AlreadyExists { get; set; }
}

public sealed class ProductVariantRowDto
{
    public int ProductId { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string? VariantLabel { get; set; }
}

public sealed class ProductTemplateProductOptionDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ProductCode { get; set; } = string.Empty;
    public int? ProductCategoryId { get; set; }
    public int? ProductTemplateId { get; set; }
}

public sealed class ProductTemplatePageDto
{
    public CreateProductTemplateDto CreateForm { get; set; } = new();
    public AssignProductTemplateDto AssignForm { get; set; } = new();
    public List<ProductTemplateRowDto> Templates { get; set; } = [];
    public List<ProductTemplateProductOptionDto> Products { get; set; } = [];
    public List<ProductCategoryDto> Categories { get; set; } = [];
    public VariantGenerationPreviewDto? Preview { get; set; }
}
