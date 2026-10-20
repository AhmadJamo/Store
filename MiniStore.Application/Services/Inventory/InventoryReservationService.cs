using MiniStore.Application.DTOs.Inventory.Reservations;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public sealed class InventoryReservationService(
    IInventoryReservationRepository reservations,
    IStorageLocationRepository locations)
{
    public sealed record Allocation(int ProductId, int WarehouseId, int? StorageLocationId,
        decimal Quantity, int? SourceLineId = null);

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
                LineCount = x.Lines.Count, TotalQuantity = x.Lines.Sum(line => line.RemainingQuantity)
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

        await ReserveAsync(InventoryReservationSourceType.StockTransfer, transfer.Id, transfer.TransferNumber,
            userId, transfer.Items.Select(item => new Allocation(item.ProductId, transfer.FromWarehouseId,
                item.SourceLocationId, item.Quantity)));
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

    public async Task<InventoryReservation> ReserveAsync(InventoryReservationSourceType sourceType,
        int sourceId, string sourceReference, string userId, IEnumerable<Allocation> requested)
    {
        var lines = requested.Where(x => x.Quantity > 0).ToList();
        if (lines.Count == 0) throw new InvalidOperationException("Reservation must contain at least one positive quantity.");
        var reservation = new InventoryReservation(sourceType, sourceId, sourceReference, userId);
        foreach (var line in lines)
        {
            if (line.StorageLocationId.HasValue)
            {
                var location = await locations.GetByIdAsync(line.StorageLocationId.Value)
                    ?? throw new InvalidOperationException("The reservation source location was not found.");
                if (location.WarehouseId != line.WarehouseId)
                    throw new InvalidOperationException("The reservation source location must belong to the selected warehouse.");
                if (location.Status != StorageLocationStatus.Active || !location.IsReservable)
                    throw new InvalidOperationException("The selected source location does not allow reservations.");
            }

            var balance = await reservations.GetBalanceForUpdateAsync(
                line.ProductId, line.WarehouseId, line.StorageLocationId)
                ?? throw new InvalidOperationException("The inventory balance required for this reservation was not found.");
            if (balance.Available < line.Quantity)
                throw new InvalidOperationException("Insufficient available stock.");
            balance.SetReserved(balance.Reserved + line.Quantity);
            reservation.AddLine(line.ProductId, line.WarehouseId, line.StorageLocationId,
                line.Quantity, line.SourceLineId);
        }
        await reservations.AddAsync(reservation);
        return reservation;
    }

    public Task ReleaseAsync(InventoryReservation reservation, string userId, string reason)
    {
        if (reservation.Status != InventoryReservationStatus.Active)
            throw new InvalidOperationException("Only an active reservation can be released.");
        return CloseAsync(reservation, consume: false, userId, reason);
    }

    public async Task ConsumeAsync(InventoryReservation reservation,
        IReadOnlyDictionary<long, decimal> quantities, string userId)
    {
        if (reservation.Status != InventoryReservationStatus.Active)
            throw new InvalidOperationException("Only an active reservation can be consumed.");
        foreach (var allocation in quantities)
        {
            var line = reservation.Lines.SingleOrDefault(x => x.Id == allocation.Key)
                ?? throw new InvalidOperationException("A reservation line does not belong to this reservation.");
            if (allocation.Value <= 0 || allocation.Value > line.RemainingQuantity)
                throw new InvalidOperationException("Consumed quantity cannot exceed the remaining reservation quantity.");
            var balance = await reservations.GetBalanceForUpdateAsync(line.ProductId, line.WarehouseId, line.StorageLocationId)
                ?? throw new InvalidOperationException("A reserved inventory balance was not found.");
            if (balance.Reserved < allocation.Value)
                throw new InvalidOperationException("The inventory reservation balance is inconsistent.");
            balance.SetReserved(balance.Reserved - allocation.Value);
        }
        reservation.Consume(quantities, userId);
    }

    private async Task CloseAsync(InventoryReservation reservation, bool consume, string userId, string? reason)
    {
        foreach (var line in reservation.Lines.Where(x => x.RemainingQuantity > 0))
        {
            var balance = await reservations.GetBalanceForUpdateAsync(line.ProductId, line.WarehouseId, line.StorageLocationId)
                ?? throw new InvalidOperationException("A reserved inventory balance was not found.");
            if (balance.Reserved < line.RemainingQuantity)
                throw new InvalidOperationException("The inventory reservation balance is inconsistent.");
            balance.SetReserved(balance.Reserved - line.RemainingQuantity);
        }
        if (consume) reservation.Consume(userId); else reservation.Release(userId, reason!);
    }
}
