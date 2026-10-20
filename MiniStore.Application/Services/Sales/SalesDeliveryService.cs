using MiniStore.Application.Dtos.Sales;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public sealed class SalesDeliveryService(
    ISalesDeliveryRepository deliveries, ISalesOrderRepository orders,
    IInventoryReservationRepository reservations, IProductRepository products,
    IProductStockRepository stocks, IWarehouseRepository warehouses, IStorageLocationRepository locations,
    IStockTransactionRepository transactions, IStockMovementRepository movements,
    IAccountingSettingsRepository accountingSettings, IBranchRepository branches,
    InventoryReservationService reservationService, InventoryTrackingService tracking,
    UntrackedInventoryRemovalService untrackedRemoval, DocumentNumberService numbers,
    JournalPostingService journalPosting, IUnitOfWork unitOfWork, ICurrentUserService currentUser)
{
    private const string JournalSourceType = "SalesDelivery";

    public async Task<List<SalesDeliverySummaryDto>> GetAllAsync(string? search)
    {
        var rows = await deliveries.GetAllAsync();
        var orderNumbers = (await orders.GetAllAsync(null, null)).ToDictionary(x => x.Id, x => x.OrderNumber);
        var warehouseNames = (await warehouses.GetAllAsync()).ToDictionary(x => x.Id, x => x.Name);
        var result = rows.Select(x => new SalesDeliverySummaryDto(x.Id, x.DeliveryNumber, x.SalesOrderId,
            orderNumbers.GetValueOrDefault(x.SalesOrderId, $"#{x.SalesOrderId}"),
            warehouseNames.GetValueOrDefault(x.WarehouseId, $"#{x.WarehouseId}"), x.DeliveryDate,
            x.Status, x.Lines.Sum(l => l.StockQuantity), x.TotalCostOfGoodsSold)).ToList();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            result = result.Where(x => x.DeliveryNumber.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                x.OrderNumber.Contains(term, StringComparison.OrdinalIgnoreCase)).ToList();
        }
        return result;
    }

    public async Task<SalesDeliveryDetailsDto?> GetAsync(int id)
    {
        var delivery = await deliveries.GetByIdAsync(id); if (delivery is null) return null;
        var order = await orders.GetByIdAsync(delivery.SalesOrderId);
        var warehouseRows = await warehouses.GetAllAsync(); var warehouseMap = warehouseRows.ToDictionary(x => x.Id, x => x.Name);
        var locationMap = (await locations.SearchAsync(null, null)).ToDictionary(x => x.Id, x => $"{x.Code} — {x.Name}");
        var progress = order is null ? new SalesDeliveryProgressDto() : await GetProgressAsync(order.Id);
        return new SalesDeliveryDetailsDto
        {
            Id = delivery.Id, DeliveryNumber = delivery.DeliveryNumber, SalesOrderId = delivery.SalesOrderId,
            OrderNumber = order?.OrderNumber ?? $"#{delivery.SalesOrderId}", InventoryReservationId = delivery.InventoryReservationId,
            WarehouseName = warehouseMap.GetValueOrDefault(delivery.WarehouseId, $"#{delivery.WarehouseId}"),
            DeliveryDate = delivery.DeliveryDate, Status = delivery.Status, Notes = delivery.Notes,
            CreatedByUserId = delivery.CreatedByUserId, CreatedAtUtc = delivery.CreatedAtUtc,
            PostedByUserId = delivery.PostedByUserId, PostedAtUtc = delivery.PostedAtUtc,
            CancellationReason = delivery.CancellationReason, RowVersion = delivery.RowVersion,
            TotalCostOfGoodsSold = delivery.TotalCostOfGoodsSold, HasRemainingDelivery = progress.RemainingStockQuantity > 0,
            Lines = delivery.Lines.Select(line => new SalesDeliveryLineDto(line.Id, line.SalesOrderLineId,
                line.InventoryReservationLineId, line.ProductCodeSnapshot, line.ProductNameSnapshot,
                warehouseMap.GetValueOrDefault(line.WarehouseId, $"#{line.WarehouseId}"),
                line.StorageLocationId.HasValue ? locationMap.GetValueOrDefault(line.StorageLocationId.Value, "Unknown location") : "Unassigned",
                line.StockQuantity, line.UnitCost, line.CostOfGoodsSold, line.TrackingAllocations)).ToList(),
            History = delivery.History.OrderBy(x => x.CreatedAtUtc).Select(x => new SalesDeliveryHistoryDto(
                x.FromStatus, x.ToStatus, x.Action, x.UserId, x.Reason, x.CreatedAtUtc)).ToList()
        };
    }

    public async Task<SalesDeliveryProgressDto> GetProgressAsync(int salesOrderId)
    {
        var order = await orders.GetByIdAsync(salesOrderId) ?? throw new InvalidOperationException("Sales order was not found.");
        var rows = await deliveries.GetBySalesOrderAsync(salesOrderId);
        var posted = rows.Where(x => x.Status == SalesDeliveryStatus.Posted).ToList();
        var required = order.Lines.Sum(x => x.StockQuantity); var delivered = posted.Sum(x => x.Lines.Sum(l => l.StockQuantity));
        return new SalesDeliveryProgressDto { RequiredStockQuantity = required, DeliveredStockQuantity = delivered,
            RemainingStockQuantity = Math.Max(0, required - delivered), IsFullyDelivered = required > 0 && delivered >= required,
            PostedDeliveryCount = posted.Count, DraftDeliveryCount = rows.Count(x => x.Status == SalesDeliveryStatus.Draft) };
    }

    public async Task<SalesDeliveryCreatePageDto?> GetCreatePageAsync(int salesOrderId)
    {
        var order = await orders.GetByIdAsync(salesOrderId); if (order is null) return null;
        var sourceReservations = (await reservations.GetAllBySourceAsync(InventoryReservationSourceType.SaleOrder, salesOrderId))
            .Where(x => x.Status == InventoryReservationStatus.Active && x.Lines.Any(l => l.RemainingQuantity > 0)).ToList();
        var existing = await deliveries.GetBySalesOrderAsync(salesOrderId);
        var openByReservation = existing.Where(x => x.Status == SalesDeliveryStatus.Draft)
            .ToDictionary(x => x.InventoryReservationId, x => x.Id);
        var warehouseMap = (await warehouses.GetAllAsync()).ToDictionary(x => x.Id, x => x.Name);
        var locationMap = (await locations.SearchAsync(null, null)).ToDictionary(x => x.Id, x => $"{x.Code} — {x.Name}");
        var productMap = (await products.GetAllAsync(null)).ToDictionary(x => x.Id);
        var orderLineMap = order.Lines.ToDictionary(x => x.Id);
        return new SalesDeliveryCreatePageDto
        {
            SalesOrderId = order.Id, OrderNumber = order.OrderNumber, OrderStatus = order.Status,
            Reservations = sourceReservations.Select(reservation => new SalesDeliveryReservationDto
            {
                ReservationId = reservation.Id, ReservedAt = reservation.CreatedAt,
                WarehouseName = warehouseMap.GetValueOrDefault(reservation.Lines.First().WarehouseId, "Unknown warehouse"),
                HasOpenDraft = openByReservation.ContainsKey(reservation.Id), OpenDraftId = openByReservation.GetValueOrDefault(reservation.Id),
                Lines = reservation.Lines.Where(x => x.RemainingQuantity > 0 && x.SourceLineId.HasValue).Select(line =>
                {
                    var orderLine = orderLineMap[line.SourceLineId!.Value];
                    return new SalesDeliveryReservationLineDto(line.Id, orderLine.Id, orderLine.ProductCode,
                        orderLine.ProductName, line.StorageLocationId.HasValue
                            ? locationMap.GetValueOrDefault(line.StorageLocationId.Value, "Unknown location") : "Unassigned",
                        productMap.GetValueOrDefault(line.ProductId)?.TrackingPolicy ?? ProductTrackingPolicy.None,
                        line.RemainingQuantity);
                }).ToList()
            }).ToList()
        };
    }

    public async Task<int> CreateDraftAsync(CreateSalesDeliveryDto dto)
    {
        SalesDelivery? created = null;
        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var existing = await deliveries.GetDraftByReservationAsync(dto.InventoryReservationId);
            if (existing is not null) { created = existing; return; }
            var order = await orders.GetByIdAsync(dto.SalesOrderId) ?? throw new InvalidOperationException("Sales order was not found.");
            if (order.Status != SalesOrderStatus.Confirmed) throw new InvalidOperationException("Only a confirmed non-cancelled sales order can be delivered.");
            var reservation = await reservations.GetByIdAsync(dto.InventoryReservationId)
                ?? throw new InvalidOperationException("Inventory reservation was not found.");
            if (reservation.SourceType != InventoryReservationSourceType.SaleOrder || reservation.SourceId != order.Id)
                throw new InvalidOperationException("Inventory reservation does not belong to this sales order.");
            if (reservation.Status != InventoryReservationStatus.Active)
                throw new InvalidOperationException("Only an active sales order reservation can be delivered.");
            if (dto.Lines.Count == 0 || dto.Lines.GroupBy(x => x.InventoryReservationLineId).Any(x => x.Count() > 1))
                throw new ArgumentException("Delivery lines are required and cannot be duplicated.");
            var selected = dto.Lines.Where(x => x.StockQuantity > 0).ToList();
            if (selected.Count == 0) throw new ArgumentException("Enter at least one positive delivery quantity.");
            var warehouseIds = reservation.Lines.Select(x => x.WarehouseId).Distinct().ToList();
            if (warehouseIds.Count != 1) throw new InvalidOperationException("A delivery reservation must use one warehouse.");
            var orderLines = order.Lines.ToDictionary(x => x.Id);
            created = new SalesDelivery(await numbers.GenerateAsync(DocumentNumberType.SalesDelivery,
                dto.DeliveryDate.ToDateTime(TimeOnly.MinValue)), order.Id, reservation.Id, warehouseIds[0],
                dto.DeliveryDate, dto.Notes, currentUser.UserId);
            foreach (var input in selected)
            {
                var reservationLine = reservation.Lines.SingleOrDefault(x => x.Id == input.InventoryReservationLineId)
                    ?? throw new InvalidOperationException("A reservation line does not belong to the selected reservation.");
                if (!reservationLine.SourceLineId.HasValue || !orderLines.TryGetValue(reservationLine.SourceLineId.Value, out var orderLine))
                    throw new InvalidOperationException("A reservation line is not linked to this sales order.");
                if (input.StockQuantity > reservationLine.RemainingQuantity)
                    throw new InvalidOperationException("Delivery quantity cannot exceed the remaining reservation quantity.");
                created.AddLine(new SalesDeliveryLine(orderLine.Id, reservationLine.Id, orderLine.ProductId,
                    reservationLine.WarehouseId, reservationLine.StorageLocationId, input.StockQuantity,
                    orderLine.ProductCode, orderLine.ProductName, input.TrackingAllocations));
            }
            await deliveries.AddAsync(created);
        });
        return created!.Id;
    }

    public Task PostAsync(SalesDeliveryStatusCommandDto dto) => unitOfWork.ExecuteInTransactionAsync(async () =>
    {
        var delivery = await deliveries.GetByIdAsync(dto.Id) ?? throw new InvalidOperationException("Sales delivery was not found.");
        EnsureVersion(delivery.RowVersion, dto.RowVersion);
        if (delivery.Status != SalesDeliveryStatus.Draft) throw new InvalidOperationException("Only a draft sales delivery can be posted.");
        var order = await orders.GetByIdAsync(delivery.SalesOrderId) ?? throw new InvalidOperationException("Sales order was not found.");
        if (order.Status != SalesOrderStatus.Confirmed) throw new InvalidOperationException("Only a confirmed non-cancelled sales order can be delivered.");
        var reservation = await reservations.GetByIdAsync(delivery.InventoryReservationId)
            ?? throw new InvalidOperationException("Inventory reservation was not found.");
        if (reservation.Status != InventoryReservationStatus.Active)
            throw new InvalidOperationException("Only an active sales order reservation can be delivered.");
        var warehouse = await warehouses.GetByIdAsync(delivery.WarehouseId)
            ?? throw new InvalidOperationException("Delivery warehouse was not found.");
        if (!warehouse.InventoryAccountId.HasValue || !warehouse.BranchId.HasValue)
            throw new InvalidOperationException("Assign the delivery warehouse to a branch and inventory account before posting.");
        var settings = await accountingSettings.GetAsync()
            ?? throw new InvalidOperationException("Configure accounting posting accounts before posting sales deliveries.");
        if (!settings.CostOfSalesAccountId.HasValue)
            throw new InvalidOperationException("Configure a cost of sales account before posting sales deliveries.");
        _ = await branches.GetByIdAsync(warehouse.BranchId.Value)
            ?? throw new InvalidOperationException("Delivery warehouse branch was not found.");

        var postedByLine = (await deliveries.GetBySalesOrderAsync(order.Id))
            .Where(x => x.Status == SalesDeliveryStatus.Posted && x.Id != delivery.Id)
            .SelectMany(x => x.Lines).GroupBy(x => x.SalesOrderLineId)
            .ToDictionary(x => x.Key, x => x.Sum(y => y.StockQuantity));
        var orderLines = order.Lines.ToDictionary(x => x.Id); var reservationLines = reservation.Lines.ToDictionary(x => x.Id);
        var consume = new Dictionary<long, decimal>(); decimal totalCost = 0;
        foreach (var line in delivery.Lines)
        {
            if (!orderLines.TryGetValue(line.SalesOrderLineId, out var orderLine) ||
                postedByLine.GetValueOrDefault(line.SalesOrderLineId) + line.StockQuantity > orderLine.StockQuantity)
                throw new InvalidOperationException("Delivery quantity cannot exceed the remaining sales order quantity.");
            if (!reservationLines.TryGetValue(line.InventoryReservationLineId, out var reservationLine) ||
                reservationLine.SourceLineId != line.SalesOrderLineId || reservationLine.ProductId != line.ProductId ||
                reservationLine.WarehouseId != line.WarehouseId || reservationLine.StorageLocationId != line.StorageLocationId ||
                line.StockQuantity > reservationLine.RemainingQuantity)
                throw new InvalidOperationException("Delivery line no longer matches its remaining inventory reservation.");
            var product = await products.GetByIdAsync(line.ProductId) ?? throw new InvalidOperationException("Delivery product was not found.");
            var stock = await stocks.GetByProductAndWarehouseAsync(line.ProductId, line.WarehouseId)
                ?? throw new InvalidOperationException("Delivery warehouse stock was not found.");
            if (stock.Quantity < line.StockQuantity) throw new InvalidOperationException("Insufficient physical stock for the reserved delivery.");
            if (product.TrackingPolicy == ProductTrackingPolicy.None)
                await untrackedRemoval.RemoveReservedAsync(product, line.WarehouseId, line.StorageLocationId, line.StockQuantity);
            else
                await tracking.IssueFromPositionAsync(product, line.WarehouseId, line.StorageLocationId,
                    line.StockQuantity, delivery.DeliveryNumber, currentUser.UserId, line.TrackingAllocations);
            var cost = stock.RemoveQuantity(line.StockQuantity);
            await transactions.AddAsync(new StockTransaction(line.ProductId, line.WarehouseId,
                -line.StockQuantity, StockTransactionType.Sale, delivery.DeliveryNumber, cost));
            await movements.AddAsync(StockMovement.PostSalesDelivery(delivery.Id, line.Id, line.ProductId,
                line.WarehouseId, line.StorageLocationId, line.StockQuantity, currentUser.UserId, delivery.DeliveryNumber));
            var lineCost = Math.Abs(cost.TransactionValue); line.RecordPostedCost(cost.UnitCost, lineCost); totalCost += lineCost;
            consume[line.InventoryReservationLineId] = line.StockQuantity;
        }
        await reservationService.ConsumeAsync(reservation, consume, currentUser.UserId);
        totalCost = Math.Round(totalCost, 2, MidpointRounding.AwayFromZero);
        if (totalCost > 0)
            await journalPosting.PostAsync(new JournalPostingRequest(delivery.DeliveryDate.ToDateTime(TimeOnly.MinValue),
                $"Sales delivery {delivery.DeliveryNumber}", JournalSourceType, delivery.DeliveryNumber,
                [new JournalEntryLine(settings.CostOfSalesAccountId.Value, totalCost, 0, warehouse.BranchId, warehouse.Id,
                    $"Cost of sales for {delivery.DeliveryNumber}"),
                 new JournalEntryLine(warehouse.InventoryAccountId.Value, 0, totalCost, warehouse.BranchId, warehouse.Id,
                    $"Inventory issued for {delivery.DeliveryNumber}")],
                "This sales delivery has already been posted."));
        delivery.Post(currentUser.UserId);
    });

    public Task CancelAsync(SalesDeliveryStatusCommandDto dto) => unitOfWork.ExecuteInTransactionAsync(async () =>
    {
        var delivery = await deliveries.GetByIdAsync(dto.Id) ?? throw new InvalidOperationException("Sales delivery was not found.");
        EnsureVersion(delivery.RowVersion, dto.RowVersion); delivery.Cancel(currentUser.UserId, dto.Reason ?? string.Empty);
    });

    private static void EnsureVersion(byte[] current, byte[] submitted)
    {
        if (submitted.Length == 0 || !current.SequenceEqual(submitted))
            throw new InvalidOperationException("Sales delivery was modified by another user. Reload and try again.");
    }
}
