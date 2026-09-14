using MiniStore.Application.DTOs.LocationMovements;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public class LocationMovementService(
    ILocationMovementRepository movementRepository,
    IProductLocationStockRepository locationStockRepository,
    IStorageLocationRepository storageLocationRepository,
    IProductRepository productRepository,
    IWarehouseRepository warehouseRepository,
    ICurrentUserService currentUserService,
    IUnitOfWork unitOfWork)
{
    public async Task<LocationMovementPageDto> GetPageAsync(
        int? warehouseId,
        string? query,
        CreateLocationMovementDto? form = null)
    {
        var warehouses = (await warehouseRepository.GetAllAsync())
            .Where(warehouse => warehouse.ControlMode != InventoryControlMode.Simple)
            .ToList();
        var products = await productRepository.GetAllAsync(null);
        var locations = await storageLocationRepository.SearchAsync(null, null);
        var locationStocks = await locationStockRepository.GetAllAsync();
        var movements = await movementRepository.GetAllAsync();

        var productsById = products.ToDictionary(product => product.Id);
        var warehouseNamesById = warehouses.ToDictionary(
            warehouse => warehouse.Id,
            warehouse => warehouse.Name);
        var locationCodesById = locations.ToDictionary(
            location => location.Id,
            location => location.Code);

        var normalizedQuery = query?.Trim();
        var movementRows = movements
            .Where(movement =>
                (!warehouseId.HasValue || movement.WarehouseId == warehouseId) &&
                MatchesQuery(movement, normalizedQuery, productsById))
            .Select(movement => MapMovement(
                movement,
                productsById,
                warehouseNamesById,
                locationCodesById))
            .ToList();

        return new LocationMovementPageDto
        {
            Form = form ?? new CreateLocationMovementDto(),
            WarehouseFilterId = warehouseId,
            Query = query,
            Movements = movementRows,
            Warehouses = warehouses
                .OrderBy(warehouse => warehouse.Name)
                .Select(warehouse => new LocationMovementWarehouseDto
                {
                    Id = warehouse.Id,
                    Name = warehouse.Name
                })
                .ToList(),
            Products = products
                .OrderBy(product => product.Name)
                .Select(product => new LocationMovementProductDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    Barcode = product.Barcode
                })
                .ToList(),
            Locations = locations
                .Where(location => location.Status == StorageLocationStatus.Active)
                .Select(location => new LocationMovementLocationDto
                {
                    Id = location.Id,
                    WarehouseId = location.WarehouseId,
                    Code = location.Code
                })
                .ToList(),
            SourceStocks = locationStocks
                .Where(stock => stock.Quantity > 0)
                .Select(stock => new LocationMovementSourceStockDto
                {
                    ProductId = stock.ProductId,
                    WarehouseId = stock.WarehouseId,
                    StorageLocationId = stock.StorageLocationId,
                    Quantity = stock.Quantity
                })
                .ToList()
        };
    }

    public async Task MoveAsync(CreateLocationMovementDto dto)
    {
        ValidateInput(dto, currentUserService.UserId);

        var product = await productRepository.GetByIdAsync(dto.ProductId)
            ?? throw new InvalidOperationException("Product not found.");
        var warehouse = await warehouseRepository.GetByIdAsync(dto.WarehouseId)
            ?? throw new InvalidOperationException("Warehouse not found.");
        if (warehouse.ControlMode == InventoryControlMode.Simple)
        {
            throw new InvalidOperationException(
                "A simple warehouse does not support internal location movements.");
        }
        var sourceLocation = await storageLocationRepository
            .GetByIdAsync(dto.FromStorageLocationId)
            ?? throw new InvalidOperationException("Source location not found.");
        var destinationLocation = await storageLocationRepository
            .GetByIdAsync(dto.ToStorageLocationId)
            ?? throw new InvalidOperationException("Destination location not found.");

        ValidateLocations(
            dto.WarehouseId,
            sourceLocation,
            destinationLocation);

        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var sourceStock = await locationStockRepository.GetAsync(
                dto.ProductId,
                dto.FromStorageLocationId)
                ?? throw new InvalidOperationException(
                    "The product has no stock in the selected source location.");

            if (sourceStock.Quantity < dto.Quantity)
            {
                throw new InvalidOperationException(
                    $"Only {sourceStock.Quantity:N3} is available in the source location.");
            }

            var destinationStock = await locationStockRepository.GetAsync(
                dto.ProductId,
                dto.ToStorageLocationId);
            var allLocationStocks = await locationStockRepository.GetAllAsync();
            var destinationQuantity = allLocationStocks
                .Where(stock =>
                    stock.StorageLocationId == dto.ToStorageLocationId)
                .Sum(stock => stock.Quantity) + dto.Quantity;

            if (warehouse.EnforceLocationCapacity &&
                destinationLocation.MaximumQuantity.HasValue &&
                destinationQuantity > destinationLocation.MaximumQuantity.Value)
            {
                throw new InvalidOperationException(
                    "Quantity exceeds the destination location capacity.");
            }

            sourceStock.RemoveQuantity(dto.Quantity);

            if (destinationStock is null)
            {
                destinationStock = new ProductLocationStock(
                    dto.ProductId,
                    dto.WarehouseId,
                    dto.ToStorageLocationId);

                await locationStockRepository.AddAsync(destinationStock);
            }

            destinationStock.AddQuantity(dto.Quantity);

            var movement = new LocationMovement(
                product.Id,
                warehouse.Id,
                sourceLocation.Id,
                destinationLocation.Id,
                dto.Quantity,
                LocationMovementType.Relocation,
                currentUserService.UserId,
                dto.Reference,
                dto.Notes);

            await movementRepository.AddAsync(movement);
        });
    }

    private static void ValidateInput(
        CreateLocationMovementDto dto,
        string userId)
    {
        if (dto.WarehouseId <= 0 || dto.ProductId <= 0)
        {
            throw new ArgumentException("Warehouse and product are required.");
        }

        if (dto.FromStorageLocationId <= 0 || dto.ToStorageLocationId <= 0)
        {
            throw new ArgumentException(
                "Source and destination locations are required.");
        }

        if (dto.Quantity <= 0)
        {
            throw new ArgumentException("Quantity must be greater than zero.");
        }

        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new ArgumentException("Movement user is required.");
        }
    }

    private static void ValidateLocations(
        int warehouseId,
        StorageLocation source,
        StorageLocation destination)
    {
        if (source.Id == destination.Id)
        {
            throw new InvalidOperationException(
                "Source and destination locations must be different.");
        }

        if (source.WarehouseId != warehouseId ||
            destination.WarehouseId != warehouseId)
        {
            throw new InvalidOperationException(
                "Both locations must belong to the selected warehouse.");
        }

        if (source.Status != StorageLocationStatus.Active ||
            destination.Status != StorageLocationStatus.Active)
        {
            throw new InvalidOperationException("Both locations must be active.");
        }
    }

    private static bool MatchesQuery(
        LocationMovement movement,
        string? query,
        IReadOnlyDictionary<int, Product> productsById)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return true;
        }

        if (!productsById.TryGetValue(movement.ProductId, out var product))
        {
            return false;
        }

        return product.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
               product.Barcode.Contains(query, StringComparison.OrdinalIgnoreCase) ||
               (movement.Reference?.Contains(
                   query,
                   StringComparison.OrdinalIgnoreCase) ?? false);
    }

    private static LocationMovementDto MapMovement(
        LocationMovement movement,
        IReadOnlyDictionary<int, Product> productsById,
        IReadOnlyDictionary<int, string> warehouseNamesById,
        IReadOnlyDictionary<int, string> locationCodesById)
    {
        productsById.TryGetValue(movement.ProductId, out var product);

        return new LocationMovementDto
        {
            Id = movement.Id,
            ProductName = product?.Name ?? "Unknown Product",
            Barcode = product?.Barcode ?? string.Empty,
            WarehouseName = warehouseNamesById.GetValueOrDefault(
                movement.WarehouseId,
                "Unknown Warehouse"),
            FromLocationCode = movement.FromStorageLocationId.HasValue
                ? locationCodesById.GetValueOrDefault(
                    movement.FromStorageLocationId.Value,
                    "Unknown Location")
                : "Unassigned",
            ToLocationCode = locationCodesById.GetValueOrDefault(
                movement.ToStorageLocationId,
                "Unknown Location"),
            Quantity = movement.Quantity,
            Type = movement.Type,
            Reference = movement.Reference,
            Notes = movement.Notes,
            CreatedByUserId = movement.CreatedByUserId,
            CreatedAt = movement.CreatedAt
        };
    }
}
