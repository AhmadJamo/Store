using MiniStore.Application.DTOs.Warehouses;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public class StorageLocationService(
    IStorageLocationRepository locationRepository,
    IWarehouseRepository warehouseRepository,
    IProductStockRepository productStockRepository,
    IProductRepository productRepository)
{
    public async Task<WarehouseInventorySearchDto> SearchAsync(int? warehouseId, string? query)
    {
        var warehouses = await warehouseRepository.GetAllAsync();
        var products = await productRepository.GetAllAsync(query);
        var stocks = await productStockRepository.GetAllAsync();

        var productsById = products.ToDictionary(product => product.Id);
        var warehouseNamesById = warehouses.ToDictionary(
            warehouse => warehouse.Id,
            warehouse => warehouse.Name);

        var productRows = stocks
            .Where(stock =>
                (!warehouseId.HasValue || stock.WarehouseId == warehouseId) &&
                productsById.ContainsKey(stock.ProductId))
            .Select(stock => new WarehouseProductSearchRowDto
            {
                ProductId = stock.ProductId,
                ProductName = productsById[stock.ProductId].Name,
                Barcode = productsById[stock.ProductId].Barcode ?? string.Empty,
                WarehouseName = warehouseNamesById.GetValueOrDefault(
                    stock.WarehouseId,
                    "Unknown"),
                Quantity = stock.Quantity
            })
            .ToList();

        var locations = await locationRepository.SearchAsync(warehouseId, query);
        var (orderedLocations, depths) = OrderAsTree(locations);
        return new WarehouseInventorySearchDto
        {
            Locations = orderedLocations,
            LocationDepths = depths,
            Products = productRows
        };
    }

    public async Task CreateAsync(CreateStorageLocationDto dto)
    {
        var warehouse = await warehouseRepository.GetByIdAsync(dto.WarehouseId);
        if (warehouse is null)
        {
            throw new InvalidOperationException("Warehouse not found.");
        }

        if (warehouse.ControlMode == InventoryControlMode.Simple)
        {
            throw new InvalidOperationException(
                "Storage locations cannot be created for a simple warehouse. Change its inventory control mode first.");
        }

        var normalizedCode = dto.Code.Trim().ToUpperInvariant();
        if (await locationRepository.CodeExistsAsync(dto.WarehouseId, normalizedCode))
        {
            throw new InvalidOperationException(
                "Location code already exists in this warehouse.");
        }

        var normalizedBarcode = NormalizeBarcode(dto.Barcode);
        if (normalizedBarcode is not null &&
            await locationRepository.BarcodeExistsAsync(dto.WarehouseId, normalizedBarcode))
        {
            throw new InvalidOperationException("Location barcode already exists in this warehouse.");
        }

        await ValidateParentAsync(dto.WarehouseId, dto.ParentLocationId, null);

        var location = new StorageLocation(
            dto.WarehouseId,
            dto.Code,
            dto.Name,
            normalizedBarcode,
            dto.ParentLocationId,
            dto.Sequence,
            dto.Zone,
            dto.Aisle,
            dto.Rack,
            dto.Level,
            dto.Bin,
            dto.Type,
            dto.MaximumQuantity,
            dto.IsReceivable,
            dto.IsPickable,
            dto.IsReservable,
            dto.IsShippable,
            dto.IsCountable);

        await locationRepository.AddAsync(location);
        await locationRepository.SaveChangesAsync();
    }

    public async Task<EditStorageLocationDto?> GetForEditAsync(int id)
    {
        var location = await locationRepository.GetByIdAsync(id);
        return location is null ? null : new EditStorageLocationDto
        {
            Id = location.Id,
            WarehouseId = location.WarehouseId,
            Code = location.Code,
            Name = location.Name,
            Barcode = location.Barcode,
            ParentLocationId = location.ParentLocationId,
            Sequence = location.Sequence,
            IsReceivable = location.IsReceivable,
            IsPickable = location.IsPickable,
            IsReservable = location.IsReservable,
            IsShippable = location.IsShippable,
            IsCountable = location.IsCountable
        };
    }

    public async Task UpdateAsync(EditStorageLocationDto dto)
    {
        var location = await locationRepository.GetByIdAsync(dto.Id)
            ?? throw new InvalidOperationException("Storage location not found.");

        var normalizedBarcode = NormalizeBarcode(dto.Barcode);
        if (normalizedBarcode is not null &&
            await locationRepository.BarcodeExistsAsync(location.WarehouseId, normalizedBarcode, location.Id))
        {
            throw new InvalidOperationException("Location barcode already exists in this warehouse.");
        }

        await ValidateParentAsync(location.WarehouseId, dto.ParentLocationId, location.Id);
        location.UpdateHierarchyAndCapabilities(
            dto.Name,
            normalizedBarcode,
            dto.ParentLocationId,
            dto.Sequence,
            dto.IsReceivable,
            dto.IsPickable,
            dto.IsReservable,
            dto.IsShippable,
            dto.IsCountable);
        await locationRepository.SaveChangesAsync();
    }

    public Task<List<StorageLocation>> GetParentOptionsAsync(int warehouseId)
    {
        return locationRepository.GetWarehouseLocationsAsync(warehouseId);
    }

    private async Task ValidateParentAsync(int warehouseId, int? parentId, int? locationId)
    {
        if (!parentId.HasValue)
        {
            return;
        }

        if (parentId == locationId)
        {
            throw new InvalidOperationException("A location cannot be its own parent.");
        }

        var allLocations = await locationRepository.GetWarehouseLocationsAsync(warehouseId);
        var parent = allLocations.FirstOrDefault(item => item.Id == parentId.Value)
            ?? throw new InvalidOperationException("Parent location must belong to the same warehouse.");

        if (!locationId.HasValue)
        {
            return;
        }

        var currentId = parent.Id;
        var byId = allLocations.ToDictionary(item => item.Id);
        while (byId.TryGetValue(currentId, out var current) && current.ParentLocationId.HasValue)
        {
            if (current.ParentLocationId.Value == locationId.Value)
            {
                throw new InvalidOperationException("A location cannot be moved below one of its descendants.");
            }

            currentId = current.ParentLocationId.Value;
        }
    }

    private static string? NormalizeBarcode(string? barcode)
    {
        return string.IsNullOrWhiteSpace(barcode) ? null : barcode.Trim().ToUpperInvariant();
    }

    private static (List<StorageLocation> Locations, Dictionary<int, int> Depths) OrderAsTree(
        IReadOnlyCollection<StorageLocation> locations)
    {
        var ids = locations.Select(location => location.Id).ToHashSet();
        var children = locations
            .GroupBy(location => location.ParentLocationId.HasValue && ids.Contains(location.ParentLocationId.Value)
                ? location.ParentLocationId.Value
                : 0)
            .ToDictionary(
                group => group.Key,
                group => group.OrderBy(location => location.Sequence).ThenBy(location => location.Code).ToList());
        var ordered = new List<StorageLocation>(locations.Count);
        var depths = new Dictionary<int, int>();
        var visited = new HashSet<int>();

        void AddBranch(int parentId, int depth)
        {
            if (!children.TryGetValue(parentId, out var branch))
                return;
            foreach (var location in branch)
            {
                if (!visited.Add(location.Id))
                    continue;
                ordered.Add(location);
                depths[location.Id] = depth;
                AddBranch(location.Id, depth + 1);
            }
        }

        AddBranch(0, 0);
        foreach (var location in locations.Where(location => !visited.Contains(location.Id)))
        {
            ordered.Add(location);
            depths[location.Id] = 0;
        }

        return (ordered, depths);
    }
}
