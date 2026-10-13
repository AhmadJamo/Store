using MiniStore.Domain.Entities;

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
public sealed class CreateGoodsReceiptDraftDto
{
    public int PurchaseOrderId { get; set; }
    public DateOnly ReceiptDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    public string? Notes { get; set; }
}
public sealed class GoodsReceiptScanPageDto
{
    public GoodsReceiptDto Receipt { get; init; } = new();
    public List<GoodsReceiptScanLineDto> Lines { get; init; } = [];
    public string IdempotencyKey { get; init; } = string.Empty;
}
public sealed class GoodsReceiptScanLineDto
{
    public int PurchaseOrderLineId { get; init; }
    public string ProductCode { get; init; } = string.Empty;
    public string ProductName { get; init; } = string.Empty;
    public string? Barcode { get; init; }
    public string UnitName { get; init; } = string.Empty;
    public ProductTrackingPolicy TrackingPolicy { get; init; }
    public decimal OrderedQuantity { get; init; }
    public decimal PreviouslyPostedQuantity { get; init; }
    public decimal DraftQuantity { get; init; }
    public decimal RemainingQuantity { get; init; }
}
public sealed class ScanGoodsReceiptDto
{
    public int GoodsReceiptId { get; set; }
    public string ProductScan { get; set; } = string.Empty;
    public decimal Quantity { get; set; } = 1m;
    public string IdempotencyKey { get; set; } = string.Empty;
    public string? LotNumber { get; set; }
    public string? SerialNumbers { get; set; }
    public DateOnly? ManufactureDate { get; set; }
    public DateOnly? ExpirationDate { get; set; }
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

public sealed class GoodsReceiptDto
{
    public int Id { get; init; }
    public string ReceiptNumber { get; init; } = string.Empty;
    public string PurchaseOrderNumber { get; init; } = string.Empty;
    public string WarehouseName { get; init; } = string.Empty;
    public DateOnly ReceiptDate { get; init; }
    public string CurrencyCode { get; init; } = string.Empty;
    public string CompanyCurrencyCode { get; init; } = string.Empty;
    public decimal ExchangeRate { get; init; }
    public DateOnly ExchangeRateDate { get; init; }
    public string ExchangeRateSource { get; init; } = string.Empty;
    public GoodsReceiptStatus Status { get; init; }
    public string? Notes { get; init; }
    public List<GoodsReceiptPostedLineDto> Lines { get; init; } = [];
}
public sealed class GoodsReceiptPostedLineDto
{
    public string ProductCode { get; init; } = string.Empty;
    public string ProductName { get; init; } = string.Empty;
    public string UnitName { get; init; } = string.Empty;
    public decimal ReceivedQuantity { get; init; }
    public decimal StockQuantity { get; init; }
    public decimal UnitCost { get; init; }
    public string? LotNumber { get; init; }
    public string? SerialNumbers { get; init; }
    public DateOnly? ManufactureDate { get; init; }
    public DateOnly? ExpirationDate { get; init; }
}
