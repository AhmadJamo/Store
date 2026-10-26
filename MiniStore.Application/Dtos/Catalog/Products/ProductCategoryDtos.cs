using System.ComponentModel.DataAnnotations;

namespace MiniStore.Application.DTOs.Products;

public sealed class ProductCategoryDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Code { get; init; } = string.Empty;
    public bool IsActive { get; init; }
}

public sealed class CreateProductCategoryDto
{
    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(30)]
    [RegularExpression("^[A-Za-z0-9][A-Za-z0-9_-]*$")]
    public string Code { get; set; } = string.Empty;
}

public sealed class SetProductCategoryStatusDto
{
    public bool IsActive { get; set; }
}
