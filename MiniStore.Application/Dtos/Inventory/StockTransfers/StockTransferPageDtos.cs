using MiniStore.Domain.Entities;

namespace MiniStore.Application.DTOs.StockTransfers;

public sealed record StockTransferWarehouseOptionDto(
    int Id,
    string Name,
    InventoryControlMode ControlMode,
    bool RequireSourceLocation,
    bool RequireDestinationLocation);

public sealed record StockTransferProductOptionDto(
    int Id,
    string Name,
    string? Barcode,
    ProductTrackingPolicy TrackingPolicy);

public sealed record StockTransferLocationOptionDto(
    int Id,
    int WarehouseId,
    string Code,
    string? Barcode,
    bool IsPickable,
    bool IsReceivable);

public sealed record StockTransferFormPageDto(
    int? Id,
    string? TransferNumber,
    CreateStockTransferDto Form,
    IReadOnlyList<StockTransferWarehouseOptionDto> Warehouses,
    IReadOnlyList<StockTransferProductOptionDto> Products,
    IReadOnlyList<StockTransferLocationOptionDto> Locations);
