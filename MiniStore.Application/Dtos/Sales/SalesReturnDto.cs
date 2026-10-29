namespace MiniStore.Application.DTOs.Sales;

public sealed class SalesReturnCreatePageDto
{
    public int SaleId { get; init; }
    public string SaleInvoiceNumber { get; init; } = string.Empty;
    public DateTime Date { get; init; } = DateTime.Today;
    public List<SalesReturnCreateLineDto> Lines { get; init; } = [];
}

public sealed record SalesReturnCreateLineDto(
    int SaleItemId,
    int ProductId,
    string ProductName,
    decimal SoldQuantity,
    decimal ReturnedQuantity,
    decimal RemainingQuantity,
    MiniStore.Domain.Entities.ProductTrackingPolicy TrackingPolicy);

public class SalesReturnDto
{
    public int Id { get; set; }
    public string ReturnNumber { get; set; } = string.Empty;
    public int SaleId { get; set; }
    public string SaleInvoiceNumber { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Reason { get; set; } = string.Empty;
    public decimal RefundAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal RestockedCostAmount { get; set; }
    public List<SalesReturnItemDto> Items { get; set; } = [];
}

public class SalesReturnItemDto
{
    public int SaleItemId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal RefundAmount { get; set; }
    public bool Restocked { get; set; }
    public decimal RestockedCostAmount { get; set; }
}
