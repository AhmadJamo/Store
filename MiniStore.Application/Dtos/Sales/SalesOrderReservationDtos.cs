using System.ComponentModel.DataAnnotations;
using MiniStore.Domain.Entities;

namespace MiniStore.Application.Dtos.Sales;

public enum SalesOrderReservationShortagePolicy
{
    BlockEntireReservation = 1,
    ReserveAvailableQuantity = 2
}

public sealed class SalesOrderReservationLineInputDto
{
    [Range(1, int.MaxValue)] public int SalesOrderLineId { get; set; }
    [Range(typeof(decimal), "0", "999999999999.999999", ParseLimitsInInvariantCulture = true, ConvertValueInInvariantCulture = true)] public decimal StockQuantity { get; set; }
}

public sealed class CreateSalesOrderReservationDto
{
    [Range(1, int.MaxValue)] public int SalesOrderId { get; set; }
    [Range(1, int.MaxValue)] public int WarehouseId { get; set; }
    public int? StorageLocationId { get; set; }
    public SalesOrderReservationShortagePolicy ShortagePolicy { get; set; } = SalesOrderReservationShortagePolicy.BlockEntireReservation;
    public byte[] RowVersion { get; set; } = [];
    public List<SalesOrderReservationLineInputDto> Lines { get; set; } = [];
}

public sealed class ReleaseSalesOrderReservationDto
{
    [Range(1, int.MaxValue)] public int SalesOrderId { get; set; }
    [Range(1, long.MaxValue)] public long ReservationId { get; set; }
    [Required, StringLength(250)] public string Reason { get; set; } = string.Empty;
}

public sealed record SalesOrderReservationOptionDto(int Id, string Name);
public sealed record SalesOrderReservationLocationOptionDto(int Id, int WarehouseId, string Name);
public sealed record SalesOrderReservationPositionDto(int ProductId, string ProductName, int WarehouseId,
    string WarehouseName, int? StorageLocationId, string LocationName, decimal OnHand, decimal Reserved, decimal Available);
public sealed record SalesOrderReservationLineProgressDto(int SalesOrderLineId, string ProductCode,
    string ProductName, decimal RequiredStockQuantity, decimal ReservedStockQuantity, decimal RemainingStockQuantity);
public sealed record SalesOrderReservationAllocationDto(int SalesOrderLineId, string ProductName, decimal Quantity);
public sealed class SalesOrderReservationRecordDto
{
    public long Id { get; init; }
    public InventoryReservationStatus Status { get; init; }
    public string WarehouseName { get; init; } = string.Empty;
    public string LocationName { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime? ClosedAt { get; init; }
    public string? ReleaseReason { get; init; }
    public decimal TotalQuantity { get; init; }
    public List<SalesOrderReservationAllocationDto> Lines { get; init; } = [];
}

public sealed class SalesOrderReservationPageDto
{
    public int SalesOrderId { get; init; }
    public string OrderNumber { get; init; } = string.Empty;
    public SalesOrderStatus OrderStatus { get; init; }
    public byte[] RowVersion { get; init; } = [];
    public bool IsFullyReserved { get; init; }
    public decimal RequiredStockQuantity { get; init; }
    public decimal ReservedStockQuantity { get; init; }
    public decimal RemainingStockQuantity { get; init; }
    public List<SalesOrderReservationLineProgressDto> Lines { get; init; } = [];
    public List<SalesOrderReservationRecordDto> Reservations { get; init; } = [];
    public List<SalesOrderReservationOptionDto> Warehouses { get; init; } = [];
    public List<SalesOrderReservationLocationOptionDto> Locations { get; init; } = [];
    public List<SalesOrderReservationPositionDto> Positions { get; init; } = [];
    public CreateSalesOrderReservationDto Form { get; init; } = new();
}

public sealed class SalesOrderReservationSummaryDto
{
    public decimal RequiredStockQuantity { get; init; }
    public decimal ReservedStockQuantity { get; init; }
    public decimal RemainingStockQuantity { get; init; }
    public bool IsFullyReserved { get; init; }
    public int ActiveReservationCount { get; init; }
}
