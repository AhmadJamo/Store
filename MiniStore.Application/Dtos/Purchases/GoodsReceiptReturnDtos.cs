using MiniStore.Domain.Entities;

namespace MiniStore.Application.DTOs.Purchases;

public sealed class GoodsReceiptReturnCreatePageDto
{
    public int GoodsReceiptId { get; init; }
    public string ReceiptNumber { get; init; } = string.Empty;
    public string PurchaseOrderNumber { get; init; } = string.Empty;
    public string SupplierName { get; init; } = string.Empty;
    public string WarehouseName { get; init; } = string.Empty;
    public CreateGoodsReceiptReturnDto Form { get; init; } = new();
    public List<GoodsReceiptReturnAvailableLineDto> Lines { get; init; } = [];
}

public sealed class GoodsReceiptReturnAvailableLineDto
{
    public int GoodsReceiptLineId { get; init; }
    public string ProductCode { get; init; } = string.Empty;
    public string ProductName { get; init; } = string.Empty;
    public string UnitName { get; init; } = string.Empty;
    public decimal RemainingQuantity { get; init; }
    public string? ReceivedTracking { get; init; }
}

public sealed class CreateGoodsReceiptReturnDto
{
    public int GoodsReceiptId { get; set; }
    public DateOnly ReturnDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    public string Reason { get; set; } = string.Empty;
    public List<CreateGoodsReceiptReturnLineDto> Lines { get; set; } = [];
}

public sealed class CreateGoodsReceiptReturnLineDto
{
    public int GoodsReceiptLineId { get; set; }
    public decimal ReturnQuantity { get; set; }
    public string? TrackingAllocations { get; set; }
}

public sealed class GoodsReceiptReturnDto
{
    public int Id { get; init; }
    public string ReturnNumber { get; init; } = string.Empty;
    public string ReceiptNumber { get; init; } = string.Empty;
    public string SupplierName { get; init; } = string.Empty;
    public string WarehouseName { get; init; } = string.Empty;
    public DateOnly ReturnDate { get; init; }
    public string Reason { get; init; } = string.Empty;
    public GoodsReceiptReturnStatus Status { get; init; }
    public List<GoodsReceiptReturnLineDto> Lines { get; init; } = [];
}

public sealed class GoodsReceiptReturnLineDto
{
    public string ProductCode { get; init; } = string.Empty;
    public string ProductName { get; init; } = string.Empty;
    public string UnitName { get; init; } = string.Empty;
    public decimal ReturnQuantity { get; init; }
    public decimal StockQuantity { get; init; }
    public decimal OriginalInventoryValue { get; init; }
    public decimal RemovedInventoryCost { get; init; }
    public string? TrackingAllocations { get; init; }
}
