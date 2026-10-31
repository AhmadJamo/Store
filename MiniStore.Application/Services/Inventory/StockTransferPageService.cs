using MiniStore.Application.DTOs.StockTransfers;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public sealed class StockTransferPageService(
    IStockTransferRepository transfers,
    IWarehouseRepository warehouses,
    IProductRepository products,
    IStorageLocationRepository locations,
    IAnalyticsPrincipalDirectory principals,
    ITenantContext tenantContext)
{
    public async Task<IReadOnlyList<StockTransferDto>> GetListAsync(
        string? search,
        StockTransferStatus? status)
    {
        var rows = await transfers.GetAllAsync(search, status);
        var warehouseNames = (await warehouses.GetAllAsync()).ToDictionary(x => x.Id, x => x.Name);
        return rows.Select(x => new StockTransferDto
        {
            Id = x.Id,
            TransferNumber = x.TransferNumber,
            FromWarehouseId = x.FromWarehouseId,
            FromWarehouseName = Name(warehouseNames, x.FromWarehouseId, "Unknown Warehouse"),
            ToWarehouseId = x.ToWarehouseId,
            ToWarehouseName = Name(warehouseNames, x.ToWarehouseId, "Unknown Warehouse"),
            Status = x.Status,
            CreatedByUserId = x.CreatedByUserId,
            CreatedAt = x.CreatedAt,
            Reference = x.Reference,
            Notes = x.Notes,
            ItemsCount = x.Items.Count
        }).ToList();
    }

    public async Task<StockTransferDetailsDto?> GetDetailsAsync(int id)
    {
        var transfer = await transfers.GetByIdAsync(id);
        if (transfer is null) return null;

        var warehouseNames = (await warehouses.GetAllAsync()).ToDictionary(x => x.Id, x => x.Name);
        var productRows = await products.GetAllAsync(null);
        var productNames = productRows.ToDictionary(x => x.Id);
        var locationNames = (await locations.SearchAsync(null, null)).ToDictionary(x => x.Id, x => x.Code);
        var tenantId = tenantContext.TenantId
            ?? throw new InvalidOperationException("A tenant context is required.");
        var userNames = (await principals.GetUsersAsync(tenantId)).ToDictionary(x => x.Id, x => x.DisplayName);

        var result = new StockTransferDetailsDto
        {
            Id = transfer.Id,
            TransferNumber = transfer.TransferNumber,
            FromWarehouseId = transfer.FromWarehouseId,
            FromWarehouseName = Name(warehouseNames, transfer.FromWarehouseId, "Unknown Warehouse"),
            ToWarehouseId = transfer.ToWarehouseId,
            ToWarehouseName = Name(warehouseNames, transfer.ToWarehouseId, "Unknown Warehouse"),
            Status = transfer.Status,
            CreatedByUserId = transfer.CreatedByUserId,
            CreatedByUserName = Name(userNames, transfer.CreatedByUserId, transfer.CreatedByUserId),
            CreatedAt = transfer.CreatedAt,
            Reference = transfer.Reference,
            Notes = transfer.Notes,
            SubmittedByUserId = transfer.SubmittedByUserId,
            SubmittedByUserName = UserName(userNames, transfer.SubmittedByUserId),
            SubmittedAt = transfer.SubmittedAt,
            ApprovedByUserId = transfer.ApprovedByUserId,
            ApprovedByUserName = UserName(userNames, transfer.ApprovedByUserId),
            ApprovedAt = transfer.ApprovedAt,
            RejectedByUserId = transfer.RejectedByUserId,
            RejectedByUserName = UserName(userNames, transfer.RejectedByUserId),
            RejectedAt = transfer.RejectedAt,
            RejectionReason = transfer.RejectionReason,
            PostedByUserId = transfer.PostedByUserId,
            PostedByUserName = UserName(userNames, transfer.PostedByUserId),
            PostedAt = transfer.PostedAt,
            CancelledByUserId = transfer.CancelledByUserId,
            CancelledByUserName = UserName(userNames, transfer.CancelledByUserId),
            CancelledAt = transfer.CancelledAt,
            CancellationReason = transfer.CancellationReason
        };

        result.Items = transfer.Items.Select(item =>
        {
            productNames.TryGetValue(item.ProductId, out var product);
            return new StockTransferItemDto
            {
                Id = item.Id,
                ProductId = item.ProductId,
                ProductName = product?.Name ?? "Unknown Product",
                Barcode = product?.Barcode,
                Quantity = item.Quantity,
                SourceLocationId = item.SourceLocationId,
                DestinationLocationId = item.DestinationLocationId,
                SourceLocationCode = item.SourceLocationId.HasValue
                    ? Name(locationNames, item.SourceLocationId.Value, "Legacy / unassigned")
                    : "Legacy / unassigned",
                DestinationLocationCode = item.DestinationLocationId.HasValue
                    ? Name(locationNames, item.DestinationLocationId.Value, "Legacy / unassigned")
                    : "Legacy / unassigned",
                TrackingAllocations = item.TrackingAllocations
            };
        }).ToList();

        result.History = transfer.History.Select(x => new StockTransferHistoryDto
        {
            Id = x.Id,
            FromStatus = x.FromStatus,
            ToStatus = x.ToStatus,
            Action = x.Action,
            UserId = x.UserId,
            UserName = Name(userNames, x.UserId, x.UserId),
            Reason = x.Reason,
            CreatedAt = x.CreatedAt
        }).ToList();
        return result;
    }

    public async Task<StockTransferFormPageDto?> GetFormAsync(int? id = null)
    {
        StockTransfer? transfer = null;
        if (id.HasValue)
        {
            transfer = await transfers.GetByIdAsync(id.Value);
            if (transfer is null) return null;
            if (transfer.Status != StockTransferStatus.Draft)
                throw new InvalidOperationException("Only draft transfers can be edited.");
        }

        var warehouseRows = await warehouses.GetAllAsync();
        var productRows = await products.GetAllAsync(null);
        var locationRows = await locations.SearchAsync(null, null);
        var form = new CreateStockTransferDto
        {
            FromWarehouseId = transfer?.FromWarehouseId ?? 0,
            ToWarehouseId = transfer?.ToWarehouseId ?? 0,
            Reference = transfer?.Reference,
            Notes = transfer?.Notes,
            Items = transfer?.Items.Select(x => new CreateStockTransferItemDto
            {
                ProductId = x.ProductId,
                Quantity = x.Quantity,
                SourceLocationId = x.SourceLocationId,
                DestinationLocationId = x.DestinationLocationId,
                TrackingAllocations = x.TrackingAllocations
            }).ToList() ?? []
        };
        return new(
            transfer?.Id,
            transfer?.TransferNumber,
            form,
            warehouseRows.OrderBy(x => x.Name).Select(x => new StockTransferWarehouseOptionDto(
                x.Id, x.Name, x.ControlMode, x.RequireSourceLocationForTransfers,
                x.RequireDestinationLocationForTransfers)).ToList(),
            productRows.OrderBy(x => x.Name).Select(x => new StockTransferProductOptionDto(
                x.Id, x.Name, x.Barcode, x.TrackingPolicy)).ToList(),
            locationRows.Where(x => x.Status == StorageLocationStatus.Active).OrderBy(x => x.Code)
                .Select(x => new StockTransferLocationOptionDto(
                    x.Id, x.WarehouseId, x.Code, x.Barcode, x.IsPickable, x.IsReceivable)).ToList());
    }

    private static string UserName(IReadOnlyDictionary<string, string> values, string? id) =>
        string.IsNullOrWhiteSpace(id) ? string.Empty : Name(values, id, id);

    private static string Name<TKey>(
        IReadOnlyDictionary<TKey, string> values,
        TKey key,
        string fallback) where TKey : notnull =>
        values.TryGetValue(key, out var value) ? value : fallback;
}
