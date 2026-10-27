namespace MiniStore.Application.DTOs.ProductStocks;

public sealed record StockSelectionOptionDto(int Id, string Name);

public sealed class ProductStockFormPageDto
{
    public CreateProductStockDto Form { get; set; } = new();
    public List<StockSelectionOptionDto> Products { get; set; } = [];
    public List<StockSelectionOptionDto> Warehouses { get; set; } = [];
}

public sealed class ManualStockAdjustmentFormPageDto
{
    public CreateStockTransactionDto Form { get; set; } = new();
    public List<StockSelectionOptionDto> Products { get; set; } = [];
    public List<StockSelectionOptionDto> Warehouses { get; set; } = [];
}
