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
