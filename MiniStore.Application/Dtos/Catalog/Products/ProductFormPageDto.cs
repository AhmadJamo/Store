using MiniStore.Application.Dtos.Accounting;
using MiniStore.Application.DTOs.Settings;

namespace MiniStore.Application.DTOs.Products;

public sealed class ProductFormPageDto
{
    public ProductDto Form { get; init; } = new();
    public List<MeasurementUnitDto> MeasurementUnits { get; init; } = [];
    public List<ProductCategoryDto> Categories { get; init; } = [];
    public List<ProductSupplierOptionDto> Suppliers { get; init; } = [];
    public List<AccountDto> ExpenseAccounts { get; init; } = [];
}

public sealed record ProductSupplierOptionDto(int Id, string Name);
