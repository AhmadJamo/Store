namespace MiniStore.Application.DTOs.StockTransfers;

public sealed record ReceiveStockTransferDto(List<ReceiveStockTransferLineDto> Lines);
public sealed record ReceiveStockTransferLineDto(
    int StockTransferItemId,
    decimal ReceivedQuantity,
    decimal ShortQuantity,
    decimal DamagedQuantity,
    int? DestinationLocationId,
    string? TrackingAllocations);
