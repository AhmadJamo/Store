using MiniStore.Application.DTOs.Inventory.Scanning;
using MiniStore.Application.DTOs.LocationMovements;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public sealed class InventoryScanningService(
    IProductRepository products,
    IStorageLocationRepository locations,
    IWarehouseRepository warehouses,
    IInventoryBalanceRepository balances,
    IInventoryAdjustmentRepository adjustments,
    UnassignedStockService unassignedStock,
    LocationMovementService locationMovements,
    InventoryAdjustmentService adjustmentService,
    ICurrentUserService currentUser)
{
    public async Task<InventoryScanPageDto> ResolveAsync(InventoryScanQueryDto query)
    {
        var adjustmentToken = InventoryScanResolver.Normalize(query.AdjustmentScan);
        var productToken = InventoryScanResolver.Normalize(query.ProductScan);
        var sourceLocationToken = InventoryScanResolver.Normalize(query.SourceLocationScan);
        var locationToken = InventoryScanResolver.Normalize(query.LocationScan);
        var productRows = await products.GetAllAsync(null);
        var locationRows = await locations.SearchAsync(null, null);
        var warehouseRows = await warehouses.GetAllAsync();
        var adjustmentRows = await adjustments.GetAllAsync();

        var productMatches = string.IsNullOrEmpty(productToken)
            ? []
            : productRows.Where(x =>
                    string.Equals(x.Barcode, productToken, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(x.ProductCode, productToken, StringComparison.OrdinalIgnoreCase))
                .ToList();
        var locationMatches = string.IsNullOrEmpty(locationToken)
            ? []
            : locationRows.Where(x =>
                    string.Equals(x.Barcode, locationToken, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(x.Code, locationToken, StringComparison.OrdinalIgnoreCase))
                .ToList();
        var sourceLocationMatches = string.IsNullOrEmpty(sourceLocationToken)
            ? []
            : locationRows.Where(x =>
                    string.Equals(x.Barcode, sourceLocationToken, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(x.Code, sourceLocationToken, StringComparison.OrdinalIgnoreCase))
                .ToList();
        var adjustmentMatches = string.IsNullOrEmpty(adjustmentToken)
            ? []
            : adjustmentRows.Where(x => string.Equals(
                x.AdjustmentNumber, adjustmentToken, StringComparison.OrdinalIgnoreCase)).ToList();

        var productResult = ProductResult(productToken, productMatches);
        var adjustmentResult = AdjustmentResult(adjustmentToken, adjustmentMatches);
        var locationResult = LocationResult(locationToken, locationMatches, warehouseRows);
        var sourceLocationResult = LocationResult(sourceLocationToken, sourceLocationMatches, warehouseRows);
        var balanceRows = new List<InventoryScanBalanceDto>();
        var adjustment = adjustmentResult.Status == InventoryScanStatus.Resolved
            ? adjustmentMatches.Single()
            : null;

        if (productResult.Status == InventoryScanStatus.Resolved &&
            locationResult.Status is InventoryScanStatus.Empty or InventoryScanStatus.Resolved &&
            sourceLocationResult.Status is InventoryScanStatus.Empty or InventoryScanStatus.Resolved)
        {
            var warehouseMap = warehouseRows.ToDictionary(x => x.Id, x => x.Name);
            var locationMap = locationRows.ToDictionary(x => x.Id);
            var resolvedLocation = locationResult.Status == InventoryScanStatus.Resolved
                ? locationMatches.Single()
                : null;
            var resolvedSourceLocation = sourceLocationResult.Status == InventoryScanStatus.Resolved
                ? sourceLocationMatches.Single()
                : null;
            var selectedLocationIds = new[] { resolvedLocation?.Id, resolvedSourceLocation?.Id }
                .Where(x => x.HasValue)
                .Select(x => x!.Value)
                .ToHashSet();

            balanceRows = (await balances.GetAllAsync())
                .Where(x => x.ProductId == productResult.EntityId)
                .Where(x => selectedLocationIds.Count == 0 ||
                    x.StorageLocationId.HasValue && selectedLocationIds.Contains(x.StorageLocationId.Value))
                .Select(x => new InventoryScanBalanceDto
                {
                    WarehouseName = warehouseMap.GetValueOrDefault(x.WarehouseId, "Unknown Warehouse"),
                    LocationName = x.StorageLocationId.HasValue && locationMap.TryGetValue(x.StorageLocationId.Value, out var location)
                        ? string.IsNullOrWhiteSpace(location.Name) ? location.Code : $"{location.Code} — {location.Name}"
                        : "Unassigned",
                    IsUnassigned = !x.StorageLocationId.HasValue,
                    OnHand = x.OnHand,
                    Reserved = x.Reserved,
                    Available = x.Available
                })
                .OrderBy(x => x.WarehouseName)
                .ThenBy(x => x.LocationName)
                .ToList();
        }

        var countLocationMatches = adjustment is not null &&
            (adjustment.StorageLocationId.HasValue
                ? locationResult.Status == InventoryScanStatus.Resolved &&
                  locationResult.EntityId == adjustment.StorageLocationId
                : locationResult.Status == InventoryScanStatus.Empty);
        var canRecordCount = adjustment is not null &&
            adjustment.Status == InventoryAdjustmentStatus.Draft &&
            productResult.Status == InventoryScanStatus.Resolved &&
            sourceLocationResult.Status == InventoryScanStatus.Empty &&
            adjustment.Lines.Any(x => x.ProductId == productResult.EntityId) &&
            countLocationMatches;
        if (adjustment?.IsBlindCount == true)
            balanceRows = [];

        return new InventoryScanPageDto
        {
            Query = new InventoryScanQueryDto
            {
                AdjustmentScan = adjustmentToken,
                ProductScan = productToken,
                SourceLocationScan = sourceLocationToken,
                LocationScan = locationToken
            },
            Product = productResult,
            Adjustment = adjustmentResult,
            SourceLocation = sourceLocationResult,
            Location = locationResult,
            Balances = balanceRows,
            PutawayIdempotencyKey = InventoryScanResolver.NewPutawayIdempotencyKey(),
            RelocationIdempotencyKey = InventoryScanResolver.NewRelocationIdempotencyKey(),
            IsBlindCount = adjustment?.IsBlindCount == true,
            CanRecordCount = canRecordCount
        };
    }

    public async Task ExecutePutawayAsync(ScannedPutawayDto input)
    {
        if (input.Quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.");

        var key = InventoryScanResolver.ValidatePutawayIdempotencyKey(input.IdempotencyKey);
        var page = await ResolveAsync(new InventoryScanQueryDto
        {
            ProductScan = input.ProductScan,
            LocationScan = input.LocationScan
        });

        if (page.Product.Status != InventoryScanStatus.Resolved || !page.Product.EntityId.HasValue)
            throw new InvalidOperationException("Scan one exact product before putaway.");
        if (page.Location.Status != InventoryScanStatus.Resolved || !page.Location.EntityId.HasValue)
            throw new InvalidOperationException("Scan one exact destination location before putaway.");

        var location = await locations.GetByIdAsync(page.Location.EntityId.Value)
            ?? throw new InvalidOperationException("Storage location not found.");
        await unassignedStock.AssignAsync(
            page.Product.EntityId.Value,
            location.WarehouseId,
            location.Id,
            input.Quantity,
            key);
    }

    public async Task ExecuteRelocationAsync(ScannedRelocationDto input)
    {
        if (input.Quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.");

        var key = InventoryScanResolver.ValidateRelocationIdempotencyKey(input.IdempotencyKey);
        var page = await ResolveAsync(new InventoryScanQueryDto
        {
            ProductScan = input.ProductScan,
            SourceLocationScan = input.SourceLocationScan,
            LocationScan = input.DestinationLocationScan
        });

        if (page.Product.Status != InventoryScanStatus.Resolved || !page.Product.EntityId.HasValue)
            throw new InvalidOperationException("Scan one exact product before relocation.");
        if (page.SourceLocation.Status != InventoryScanStatus.Resolved || !page.SourceLocation.EntityId.HasValue)
            throw new InvalidOperationException("Scan one exact source location before relocation.");
        if (page.Location.Status != InventoryScanStatus.Resolved || !page.Location.EntityId.HasValue)
            throw new InvalidOperationException("Scan one exact destination location before relocation.");

        var source = await locations.GetByIdAsync(page.SourceLocation.EntityId.Value)
            ?? throw new InvalidOperationException("Source location not found.");
        await locationMovements.MoveAsync(new CreateLocationMovementDto
        {
            ProductId = page.Product.EntityId.Value,
            WarehouseId = source.WarehouseId,
            FromStorageLocationId = source.Id,
            ToStorageLocationId = page.Location.EntityId.Value,
            Quantity = input.Quantity,
            IdempotencyKey = key,
            Reference = "Inventory Scan"
        });
    }

    public async Task RecordCountAsync(ScannedInventoryCountDto input)
    {
        if (input.CountedQuantity < 0)
            throw new ArgumentException("Counted quantity cannot be negative.");

        var page = await ResolveAsync(new InventoryScanQueryDto
        {
            AdjustmentScan = input.AdjustmentNumber,
            ProductScan = input.ProductScan,
            LocationScan = input.LocationScan
        });
        if (!page.CanRecordCount || !page.Product.EntityId.HasValue || !page.Adjustment.EntityLongId.HasValue)
            throw new InvalidOperationException("The scanned count does not match an editable adjustment line and position.");

        var locationId = page.Location.Status == InventoryScanStatus.Resolved
            ? page.Location.EntityId
            : null;
        await adjustmentService.RecordScannedLineAsync(
            page.Adjustment.NormalizedValue,
            page.Product.EntityId.Value,
            locationId,
            input.CountedQuantity,
            currentUser.UserId);
    }

    private static InventoryScanResultDto ProductResult(string token, List<Product> matches)
    {
        var status = InventoryScanResolver.ResolveCount(token, matches.Count);
        var match = status == InventoryScanStatus.Resolved ? matches.Single() : null;
        return new InventoryScanResultDto
        {
            NormalizedValue = token,
            Status = status,
            EntityId = match?.Id,
            DisplayName = match?.Name ?? string.Empty,
            SecondaryText = match?.ProductCode
        };
    }

    private static InventoryScanResultDto AdjustmentResult(
        string token,
        List<InventoryAdjustment> matches)
    {
        var status = InventoryScanResolver.ResolveCount(token, matches.Count);
        var match = status == InventoryScanStatus.Resolved ? matches.Single() : null;
        return new InventoryScanResultDto
        {
            NormalizedValue = token,
            Status = status,
            EntityLongId = match?.Id,
            DisplayName = match?.AdjustmentNumber ?? string.Empty,
            SecondaryText = match?.Status.ToString()
        };
    }

    private static InventoryScanResultDto LocationResult(
        string token,
        List<StorageLocation> matches,
        List<Warehouse> warehouses)
    {
        var status = InventoryScanResolver.ResolveCount(token, matches.Count);
        var match = status == InventoryScanStatus.Resolved ? matches.Single() : null;
        var warehouseMap = warehouses.ToDictionary(x => x.Id, x => x.Name);
        return new InventoryScanResultDto
        {
            NormalizedValue = token,
            Status = status,
            EntityId = match?.Id,
            DisplayName = match is null ? string.Empty : $"{match.Code} — {match.Name}",
            SecondaryText = match is null ? null : warehouseMap.GetValueOrDefault(match.WarehouseId, "Unknown Warehouse")
        };
    }
}
