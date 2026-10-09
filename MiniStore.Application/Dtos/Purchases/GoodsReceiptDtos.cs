namespace MiniStore.Application.DTOs.Purchases;

public sealed class GoodsReceiptCreatePageDto
{
    public PurchaseOrderDto? PurchaseOrder { get; init; }
    public CreateGoodsReceiptDto Form { get; init; } = new();
    public List<GoodsReceiptRemainingLineDto> RemainingLines { get; init; } = [];
}
public sealed class GoodsReceiptRemainingLineDto
{
    public int PurchaseOrderLineId { get; init; }
    public string ProductCode { get; init; } = string.Empty;
    public string ProductName { get; init; } = string.Empty;
    public string UnitName { get; init; } = string.Empty;
    public decimal RemainingQuantity { get; init; }
}
public sealed class CreateGoodsReceiptDto
{
    public int PurchaseOrderId { get; set; }
    public DateOnly ReceiptDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    public string? Notes { get; set; }
    public List<CreateGoodsReceiptLineDto> Lines { get; set; } = [];
}
public sealed class CreateGoodsReceiptLineDto
{
    public int PurchaseOrderLineId { get; set; }
    public decimal ReceivedQuantity { get; set; }
    public string? LotNumber { get; set; }
    public string? SerialNumbers { get; set; }
    public DateOnly? ManufactureDate { get; set; }
    public DateOnly? ExpirationDate { get; set; }
}
