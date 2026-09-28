using MiniStore.Domain.Entities;

namespace MiniStore.Application.DTOs.Products;

public sealed class ProductListQueryDto
{
    public string? Search { get; set; }
    public ProductType? ProductType { get; set; }
    public bool? IsSellableInPos { get; set; }
    public bool? IsSellableInSales { get; set; }
    public bool? IsActive { get; set; }
    public string SortBy { get; set; } = "name";
    public bool SortDescending { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 25;
}

public sealed class ProductListPageDto
{
    public ProductListQueryDto Query { get; init; } = new();
    public List<ProductDto> Items { get; init; } = [];
    public int TotalCount { get; init; }
    public int TotalPages { get; init; }
}
