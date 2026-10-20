using MiniStore.Application.Dtos.Sales;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public sealed class SalesOrderReservationService(
    ISalesOrderRepository orders,
    IInventoryReservationRepository reservations,
    IInventoryBalanceRepository balances,
    IWarehouseRepository warehouses,
    IStorageLocationRepository locations,
    InventoryReservationService reservationService,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser)
{
    public async Task<SalesOrderReservationSummaryDto> GetSummaryAsync(int salesOrderId)
    {
        var order = await orders.GetByIdAsync(salesOrderId)
            ?? throw new InvalidOperationException("Sales order was not found.");
        var sourceReservations = await reservations.GetAllBySourceAsync(
            InventoryReservationSourceType.SaleOrder, salesOrderId);
        return BuildSummary(order, sourceReservations);
    }

    public async Task<SalesOrderReservationPageDto?> GetPageAsync(int salesOrderId)
    {
        var order = await orders.GetByIdAsync(salesOrderId);
        if (order is null) return null;

        var sourceReservations = await reservations.GetAllBySourceAsync(
            InventoryReservationSourceType.SaleOrder, salesOrderId);
        var warehouseRows = await warehouses.GetAllAsync();
        var locationRows = await locations.SearchAsync(null, null);
        var balanceRows = await balances.GetAllAsync();
        var warehouseMap = warehouseRows.ToDictionary(x => x.Id, x => x.Name);
        var locationMap = locationRows.ToDictionary(x => x.Id,
            x => string.IsNullOrWhiteSpace(x.Name) ? x.Code : $"{x.Code} — {x.Name}");
        var lineMap = order.Lines.ToDictionary(x => x.Id);
        var activeByLine = ActiveReservedByLine(sourceReservations);
        var progress = order.Lines.Select(line =>
        {
            var reserved = activeByLine.GetValueOrDefault(line.Id);
            return new SalesOrderReservationLineProgressDto(line.Id, line.ProductCode, line.ProductName,
                line.StockQuantity, reserved, Math.Max(0, line.StockQuantity - reserved));
        }).ToList();
        var summary = BuildSummary(order, sourceReservations);
        var productIds = order.Lines.Select(x => x.ProductId).ToHashSet();
        var productNames = order.Lines.GroupBy(x => x.ProductId)
            .ToDictionary(x => x.Key, x => x.First().ProductName);

        return new SalesOrderReservationPageDto
        {
            SalesOrderId = order.Id,
            OrderNumber = order.OrderNumber,
            OrderStatus = order.Status,
            RowVersion = order.RowVersion,
            IsFullyReserved = summary.IsFullyReserved,
            RequiredStockQuantity = summary.RequiredStockQuantity,
            ReservedStockQuantity = summary.ReservedStockQuantity,
            RemainingStockQuantity = summary.RemainingStockQuantity,
            Lines = progress,
            Reservations = sourceReservations.Select(reservation =>
            {
                var first = reservation.Lines.FirstOrDefault();
                var oneWarehouse = reservation.Lines.Select(x => x.WarehouseId).Distinct().Count() <= 1;
                var oneLocation = reservation.Lines.Select(x => x.StorageLocationId).Distinct().Count() <= 1;
                return new SalesOrderReservationRecordDto
                {
                    Id = reservation.Id,
                    Status = reservation.Status,
                    WarehouseName = oneWarehouse && first is not null
                        ? warehouseMap.GetValueOrDefault(first.WarehouseId, "Unknown warehouse") : "Multiple warehouses",
                    LocationName = oneLocation && first is not null
                        ? first.StorageLocationId.HasValue
                            ? locationMap.GetValueOrDefault(first.StorageLocationId.Value, "Unknown location")
                            : "Unassigned"
                        : "Multiple locations",
                    CreatedAt = reservation.CreatedAt,
                    ClosedAt = reservation.ClosedAt,
                    ReleaseReason = reservation.ReleaseReason,
                    TotalQuantity = reservation.Lines.Sum(x => x.Quantity),
                    Lines = reservation.Lines.Select(line => new SalesOrderReservationAllocationDto(
                        line.SourceLineId ?? 0,
                        line.SourceLineId.HasValue && lineMap.TryGetValue(line.SourceLineId.Value, out var orderLine)
                            ? orderLine.ProductName : productNames.GetValueOrDefault(line.ProductId, "Unknown product"),
                        line.Quantity)).ToList()
                };
            }).ToList(),
            Warehouses = warehouseRows.OrderBy(x => x.Name)
                .Select(x => new SalesOrderReservationOptionDto(x.Id, x.Name)).ToList(),
            Locations = locationRows.Where(x => x.Status == StorageLocationStatus.Active && x.IsReservable)
                .OrderBy(x => warehouseMap.GetValueOrDefault(x.WarehouseId)).ThenBy(x => x.Sequence).ThenBy(x => x.Code)
                .Select(x => new SalesOrderReservationLocationOptionDto(x.Id, x.WarehouseId,
                    $"{warehouseMap.GetValueOrDefault(x.WarehouseId, "Unknown warehouse")} — {locationMap[x.Id]}"))
                .ToList(),
            Positions = balanceRows.Where(x => productIds.Contains(x.ProductId))
                .OrderBy(x => productNames.GetValueOrDefault(x.ProductId))
                .ThenBy(x => warehouseMap.GetValueOrDefault(x.WarehouseId))
                .ThenBy(x => x.StorageLocationId.HasValue ? locationMap.GetValueOrDefault(x.StorageLocationId.Value) : string.Empty)
                .Select(x => new SalesOrderReservationPositionDto(x.ProductId,
                    productNames.GetValueOrDefault(x.ProductId, "Unknown product"), x.WarehouseId,
                    warehouseMap.GetValueOrDefault(x.WarehouseId, "Unknown warehouse"), x.StorageLocationId,
                    x.StorageLocationId.HasValue ? locationMap.GetValueOrDefault(x.StorageLocationId.Value, "Unknown location") : "Unassigned",
                    x.OnHand, x.Reserved, x.Available)).ToList(),
            Form = new CreateSalesOrderReservationDto
            {
                SalesOrderId = order.Id,
                RowVersion = order.RowVersion,
                Lines = progress.Select(x => new SalesOrderReservationLineInputDto
                    { SalesOrderLineId = x.SalesOrderLineId, StockQuantity = x.RemainingStockQuantity }).ToList()
            }
        };
    }

    public Task ReserveAsync(CreateSalesOrderReservationDto dto) => unitOfWork.ExecuteInTransactionAsync(async () =>
    {
        if (!Enum.IsDefined(dto.ShortagePolicy))
            throw new ArgumentException("Selected reservation shortage policy is invalid.");
        var order = await orders.GetByIdAsync(dto.SalesOrderId)
            ?? throw new InvalidOperationException("Sales order was not found.");
        EnsureVersion(order.RowVersion, dto.RowVersion);
        if (order.Status != SalesOrderStatus.Confirmed)
            throw new InvalidOperationException("Only a confirmed non-cancelled sales order can reserve inventory.");
        var warehouse = await warehouses.GetByIdAsync(dto.WarehouseId)
            ?? throw new InvalidOperationException("Selected reservation warehouse was not found.");
        if (dto.StorageLocationId.HasValue)
        {
            var location = await locations.GetByIdAsync(dto.StorageLocationId.Value)
                ?? throw new InvalidOperationException("The reservation source location was not found.");
            if (location.WarehouseId != warehouse.Id)
                throw new InvalidOperationException("The reservation source location must belong to the selected warehouse.");
            if (location.Status != StorageLocationStatus.Active || !location.IsReservable)
                throw new InvalidOperationException("The selected source location does not allow reservations.");
        }
        if (dto.Lines.Count == 0 || dto.Lines.GroupBy(x => x.SalesOrderLineId).Any(x => x.Count() > 1))
            throw new ArgumentException("Reservation lines are required and cannot be duplicated.");

        var active = (await reservations.GetAllBySourceAsync(InventoryReservationSourceType.SaleOrder, order.Id))
            .Where(x => x.Status == InventoryReservationStatus.Active).ToList();
        var reservedByLine = ActiveReservedByLine(active);
        var orderLines = order.Lines.ToDictionary(x => x.Id);
        var plannedByProduct = new Dictionary<int, decimal>();
        var allocations = new List<InventoryReservationService.Allocation>();

        foreach (var input in dto.Lines.Where(x => x.StockQuantity > 0))
        {
            if (!orderLines.TryGetValue(input.SalesOrderLineId, out var orderLine))
                throw new InvalidOperationException("A reservation line does not belong to this sales order.");
            var remaining = orderLine.StockQuantity - reservedByLine.GetValueOrDefault(orderLine.Id);
            if (remaining <= 0) continue;
            if (input.StockQuantity > remaining)
                throw new InvalidOperationException("Reservation quantity cannot exceed the remaining sales order quantity.");

            var balance = await reservations.GetBalanceForUpdateAsync(orderLine.ProductId,
                warehouse.Id, dto.StorageLocationId);
            var available = Math.Max(0, (balance?.Available ?? 0) - plannedByProduct.GetValueOrDefault(orderLine.ProductId));
            var quantity = dto.ShortagePolicy == SalesOrderReservationShortagePolicy.ReserveAvailableQuantity
                ? Math.Min(input.StockQuantity, available)
                : input.StockQuantity;
            if (dto.ShortagePolicy == SalesOrderReservationShortagePolicy.BlockEntireReservation && available < quantity)
                throw new InvalidOperationException("Insufficient available stock.");
            if (quantity <= 0) continue;
            plannedByProduct[orderLine.ProductId] = plannedByProduct.GetValueOrDefault(orderLine.ProductId) + quantity;
            allocations.Add(new InventoryReservationService.Allocation(orderLine.ProductId, warehouse.Id,
                dto.StorageLocationId, quantity, orderLine.Id));
        }
        if (allocations.Count == 0)
            throw new InvalidOperationException("No available quantity can be reserved for the selected sales order lines.");

        await reservationService.ReserveAsync(InventoryReservationSourceType.SaleOrder, order.Id,
            order.OrderNumber, currentUser.UserId, allocations);
    });

    public Task ReleaseAsync(ReleaseSalesOrderReservationDto dto) => unitOfWork.ExecuteInTransactionAsync(async () =>
    {
        var reservation = await reservations.GetByIdAsync(dto.ReservationId)
            ?? throw new InvalidOperationException("Inventory reservation was not found.");
        if (reservation.SourceType != InventoryReservationSourceType.SaleOrder ||
            reservation.SourceId != dto.SalesOrderId)
            throw new InvalidOperationException("Inventory reservation does not belong to this sales order.");
        await reservationService.ReleaseAsync(reservation, currentUser.UserId, dto.Reason);
    });

    public async Task ReleaseAllActiveAsync(int salesOrderId, string reason)
    {
        var active = (await reservations.GetAllBySourceAsync(InventoryReservationSourceType.SaleOrder, salesOrderId))
            .Where(x => x.Status == InventoryReservationStatus.Active).ToList();
        foreach (var reservation in active)
            await reservationService.ReleaseAsync(reservation, currentUser.UserId, reason);
    }

    private static Dictionary<int, decimal> ActiveReservedByLine(IEnumerable<InventoryReservation> source) =>
        source.Where(x => x.Status == InventoryReservationStatus.Active)
            .SelectMany(x => x.Lines).Where(x => x.SourceLineId.HasValue)
            .GroupBy(x => x.SourceLineId!.Value).ToDictionary(x => x.Key, x => x.Sum(y => y.Quantity));

    private static SalesOrderReservationSummaryDto BuildSummary(SalesOrder order,
        IEnumerable<InventoryReservation> sourceReservations)
    {
        var reservedByLine = ActiveReservedByLine(sourceReservations);
        var required = order.Lines.Sum(x => x.StockQuantity);
        var reserved = order.Lines.Sum(x => Math.Min(x.StockQuantity, reservedByLine.GetValueOrDefault(x.Id)));
        return new SalesOrderReservationSummaryDto
        {
            RequiredStockQuantity = required,
            ReservedStockQuantity = reserved,
            RemainingStockQuantity = Math.Max(0, required - reserved),
            IsFullyReserved = required > 0 && reserved >= required,
            ActiveReservationCount = sourceReservations.Count(x => x.Status == InventoryReservationStatus.Active)
        };
    }

    private static void EnsureVersion(byte[] current, byte[] submitted)
    {
        if (submitted.Length == 0 || !current.SequenceEqual(submitted))
            throw new InvalidOperationException("Sales order was modified by another user. Reload and try again.");
    }
}
