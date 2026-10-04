using MiniStore.Application.DTOs.Inventory.Reservations;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public sealed class InventoryReservationService(
    IInventoryReservationRepository reservations,
    IStorageLocationRepository locations)
{
    public async Task<InventoryReservationPageDto> GetPageAsync(InventoryReservationStatus? status, string? search)
    {
        var normalized = search?.Trim() ?? string.Empty;
        var rows = (await reservations.GetAllAsync())
            .Where(x => !status.HasValue || x.Status == status)
            .Where(x => normalized.Length == 0 || x.SourceReference.Contains(normalized, StringComparison.OrdinalIgnoreCase))
            .Select(x => new InventoryReservationRowDto
            {
                Id = x.Id, SourceType = x.SourceType, SourceReference = x.SourceReference,
                Status = x.Status, CreatedAt = x.CreatedAt, ClosedAt = x.ClosedAt,
                LineCount = x.Lines.Count, TotalQuantity = x.Lines.Sum(line => line.Quantity)
            }).ToList();
        return new InventoryReservationPageDto { Rows = rows, Status = status, Search = normalized };
    }

    public async Task ReserveTransferAsync(StockTransfer transfer, string userId)
    {
        var existing = await reservations.GetBySourceAsync(InventoryReservationSourceType.StockTransfer, transfer.Id);
        if (existing is not null)
        {
            if (existing.Status == InventoryReservationStatus.Active) return;
            throw new InvalidOperationException("The transfer reservation has already been closed.");
        }

        var reservation = new InventoryReservation(InventoryReservationSourceType.StockTransfer,
            transfer.Id, transfer.TransferNumber, userId);
        foreach (var item in transfer.Items)
        {
            if (item.SourceLocationId.HasValue)
            {
                var location = await locations.GetByIdAsync(item.SourceLocationId.Value)
                    ?? throw new InvalidOperationException("The reservation source location was not found.");
                if (!location.IsReservable)
                    throw new InvalidOperationException("The selected source location does not allow reservations.");
            }

            var balance = await reservations.GetBalanceForUpdateAsync(
                item.ProductId, transfer.FromWarehouseId, item.SourceLocationId)
                ?? throw new InvalidOperationException("The inventory balance required for this reservation was not found.");
            if (balance.Available < item.Quantity)
                throw new InvalidOperationException(
                    $"Insufficient available stock for product ID {item.ProductId}. Available: {balance.Available}, Requested: {item.Quantity}.");
            balance.SetReserved(balance.Reserved + item.Quantity);
            reservation.AddLine(item.ProductId, transfer.FromWarehouseId, item.SourceLocationId, item.Quantity);
        }
        await reservations.AddAsync(reservation);
    }

    public async Task ConsumeTransferAsync(int transferId, string userId)
    {
        var reservation = await reservations.GetBySourceAsync(InventoryReservationSourceType.StockTransfer, transferId)
            ?? throw new InvalidOperationException("The active transfer reservation was not found.");
        if (reservation.Status == InventoryReservationStatus.Consumed) return;
        if (reservation.Status != InventoryReservationStatus.Active)
            throw new InvalidOperationException("The transfer reservation is not active.");
        await CloseAsync(reservation, consume: true, userId, null);
    }

    public async Task ReleaseTransferAsync(int transferId, string userId, string reason)
    {
        var reservation = await reservations.GetBySourceAsync(InventoryReservationSourceType.StockTransfer, transferId);
        if (reservation is null || reservation.Status == InventoryReservationStatus.Released) return;
        if (reservation.Status != InventoryReservationStatus.Active)
            throw new InvalidOperationException("Only an active transfer reservation can be released.");
        await CloseAsync(reservation, consume: false, userId, reason);
    }

    private async Task CloseAsync(InventoryReservation reservation, bool consume, string userId, string? reason)
    {
        foreach (var line in reservation.Lines)
        {
            var balance = await reservations.GetBalanceForUpdateAsync(line.ProductId, line.WarehouseId, line.StorageLocationId)
                ?? throw new InvalidOperationException("A reserved inventory balance was not found.");
            if (balance.Reserved < line.Quantity)
                throw new InvalidOperationException("The inventory reservation balance is inconsistent.");
            balance.SetReserved(balance.Reserved - line.Quantity);
        }
        if (consume) reservation.Consume(userId); else reservation.Release(userId, reason!);
    }
}
