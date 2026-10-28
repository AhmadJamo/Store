using System.ComponentModel.DataAnnotations;
using MiniStore.Domain.Entities;

namespace MiniStore.Application.DTOs.Purchases;

public sealed class GoodsReceiptCreatePageDto
{
    public PurchaseOrderDto? PurchaseOrder { get; init; }
    public PurchaseReceivingRoute ReceivingRoute { get; init; } = PurchaseReceivingRoute.OneStep;
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
    [Range(1, int.MaxValue)]
    public int PurchaseOrderId { get; set; }
    public DateOnly ReceiptDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    [StringLength(1000)]
    public string? Notes { get; set; }
    [MinLength(1)]
    public List<CreateGoodsReceiptLineDto> Lines { get; set; } = [];
}
public sealed class CreateGoodsReceiptDraftDto
{
    [Range(1, int.MaxValue)]
    public int PurchaseOrderId { get; set; }
    public DateOnly ReceiptDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    [StringLength(1000)]
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
    [Range(1, int.MaxValue)]
    public int GoodsReceiptId { get; set; }
    [Required, StringLength(100)]
    public string ProductScan { get; set; } = string.Empty;
    [Range(typeof(decimal), "0.000001", "999999999999", ParseLimitsInInvariantCulture = true, ConvertValueInInvariantCulture = true)]
    public decimal Quantity { get; set; } = 1m;
    [Required, StringLength(100, MinimumLength = 16)]
    public string IdempotencyKey { get; set; } = string.Empty;
    [StringLength(100)]
    public string? LotNumber { get; set; }
    [StringLength(2000)]
    public string? SerialNumbers { get; set; }
    public DateOnly? ManufactureDate { get; set; }
    public DateOnly? ExpirationDate { get; set; }
}
public sealed class CreateGoodsReceiptLineDto
{
    [Range(1, int.MaxValue)]
    public int PurchaseOrderLineId { get; set; }
    [Range(typeof(decimal), "0", "999999999999", ParseLimitsInInvariantCulture = true, ConvertValueInInvariantCulture = true)]
    public decimal ReceivedQuantity { get; set; }
    [StringLength(100)]
    public string? LotNumber { get; set; }
    [StringLength(2000)]
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
    public PurchaseReceivingRoute ReceivingRoute { get; init; }
    public string? InspectedByUserId { get; init; }
    public DateTime? InspectedAtUtc { get; init; }
    public string? InspectionNotes { get; init; }
    public string? Notes { get; init; }
    public List<GoodsReceiptPostedLineDto> Lines { get; init; } = [];
}
public sealed class GoodsReceiptPostedLineDto
{
    public int Id { get; init; }
    public string ProductCode { get; init; } = string.Empty;
    public string ProductName { get; init; } = string.Empty;
    public string UnitName { get; init; } = string.Empty;
    public decimal ReceivedQuantity { get; init; }
    public decimal DeliveredQuantity { get; init; }
    public decimal AcceptedQuantity { get; init; }
    public decimal RejectedQuantity { get; init; }
    public decimal DamagedQuantity { get; init; }
    public decimal ShortQuantity { get; init; }
    public string? DiscrepancyReason { get; init; }
    public decimal StockQuantity { get; init; }
    public decimal UnitCost { get; init; }
    public string? LotNumber { get; init; }
    public string? SerialNumbers { get; init; }
    public DateOnly? ManufactureDate { get; init; }
    public DateOnly? ExpirationDate { get; init; }
}

public sealed class InspectGoodsReceiptDto
{
    [Range(1, int.MaxValue)]
    public int GoodsReceiptId { get; set; }
    [StringLength(1000)]
    public string? Notes { get; set; }
    [MinLength(1)]
    public List<InspectGoodsReceiptLineDto> Lines { get; set; } = [];
}

public sealed class InspectGoodsReceiptLineDto
{
    [Range(1, int.MaxValue)]
    public int GoodsReceiptLineId { get; set; }
    [Range(typeof(decimal), "0", "999999999999", ParseLimitsInInvariantCulture = true, ConvertValueInInvariantCulture = true)]
    public decimal AcceptedQuantity { get; set; }
    [Range(typeof(decimal), "0", "999999999999", ParseLimitsInInvariantCulture = true, ConvertValueInInvariantCulture = true)]
    public decimal RejectedQuantity { get; set; }
    [Range(typeof(decimal), "0", "999999999999", ParseLimitsInInvariantCulture = true, ConvertValueInInvariantCulture = true)]
    public decimal DamagedQuantity { get; set; }
    [Range(typeof(decimal), "0", "999999999999", ParseLimitsInInvariantCulture = true, ConvertValueInInvariantCulture = true)]
    public decimal ShortQuantity { get; set; }
    [StringLength(500)]
    public string? Reason { get; set; }
}
