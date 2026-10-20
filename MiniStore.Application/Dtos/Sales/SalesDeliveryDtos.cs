using System.ComponentModel.DataAnnotations;
using MiniStore.Domain.Entities;

namespace MiniStore.Application.Dtos.Sales;

public sealed record SalesDeliverySummaryDto(int Id, string DeliveryNumber, int SalesOrderId,
    string OrderNumber, string WarehouseName, DateOnly DeliveryDate, SalesDeliveryStatus Status,
    decimal StockQuantity, decimal CostOfGoodsSold);

public sealed record SalesDeliveryLineDto(int Id, int SalesOrderLineId, long InventoryReservationLineId,
    string ProductCode, string ProductName, string WarehouseName, string LocationName,
    decimal StockQuantity, decimal UnitCost, decimal CostOfGoodsSold, string? TrackingAllocations);
public sealed record SalesDeliveryHistoryDto(SalesDeliveryStatus FromStatus, SalesDeliveryStatus ToStatus,
    SalesDeliveryHistoryAction Action, string UserId, string? Reason, DateTime CreatedAtUtc);

public sealed class SalesDeliveryDetailsDto
{
    public int Id { get; init; } public string DeliveryNumber { get; init; } = string.Empty;
    public int SalesOrderId { get; init; } public string OrderNumber { get; init; } = string.Empty;
    public long InventoryReservationId { get; init; } public string WarehouseName { get; init; } = string.Empty;
    public DateOnly DeliveryDate { get; init; } public SalesDeliveryStatus Status { get; init; }
    public string? Notes { get; init; } public string CreatedByUserId { get; init; } = string.Empty;
    public DateTime CreatedAtUtc { get; init; } public string? PostedByUserId { get; init; }
    public DateTime? PostedAtUtc { get; init; } public string? CancellationReason { get; init; }
    public byte[] RowVersion { get; init; } = []; public decimal TotalCostOfGoodsSold { get; init; }
    public List<SalesDeliveryLineDto> Lines { get; init; } = [];
    public List<SalesDeliveryHistoryDto> History { get; init; } = [];
    public bool HasRemainingDelivery { get; init; }
}

public sealed class CreateSalesDeliveryLineDto
{
    [Range(1, long.MaxValue)] public long InventoryReservationLineId { get; set; }
    [Range(typeof(decimal), "0", "999999999999.999999")] public decimal StockQuantity { get; set; }
    [StringLength(4000)] public string? TrackingAllocations { get; set; }
}
public sealed class CreateSalesDeliveryDto
{
    [Range(1, int.MaxValue)] public int SalesOrderId { get; set; }
    [Range(1, long.MaxValue)] public long InventoryReservationId { get; set; }
    public DateOnly DeliveryDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    [StringLength(1000)] public string? Notes { get; set; }
    public List<CreateSalesDeliveryLineDto> Lines { get; set; } = [];
}
public sealed record SalesDeliveryReservationLineDto(long InventoryReservationLineId, int SalesOrderLineId,
    string ProductCode, string ProductName, string LocationName, ProductTrackingPolicy TrackingPolicy,
    decimal RemainingQuantity);
public sealed class SalesDeliveryReservationDto
{
    public long ReservationId { get; init; } public string WarehouseName { get; init; } = string.Empty;
    public DateTime ReservedAt { get; init; } public bool HasOpenDraft { get; init; }
    public int? OpenDraftId { get; init; } public List<SalesDeliveryReservationLineDto> Lines { get; init; } = [];
}
public sealed class SalesDeliveryCreatePageDto
{
    public int SalesOrderId { get; init; } public string OrderNumber { get; init; } = string.Empty;
    public SalesOrderStatus OrderStatus { get; init; } public List<SalesDeliveryReservationDto> Reservations { get; init; } = [];
}
public sealed class SalesDeliveryStatusCommandDto
{
    [Range(1, int.MaxValue)] public int Id { get; set; } public byte[] RowVersion { get; set; } = [];
    [StringLength(500)] public string? Reason { get; set; }
}
public sealed class SalesDeliveryProgressDto
{
    public decimal RequiredStockQuantity { get; init; }
    public decimal DeliveredStockQuantity { get; init; }
    public decimal RemainingStockQuantity { get; init; }
    public bool IsFullyDelivered { get; init; }
    public int PostedDeliveryCount { get; init; }
    public int DraftDeliveryCount { get; init; }
}
