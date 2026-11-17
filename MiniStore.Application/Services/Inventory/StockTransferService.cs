using MiniStore.Application.Exceptions;
using MiniStore.Domain.Commands;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public class StockTransferService : IStockTransferService
{
    private readonly IStockTransferRepository _stockTransferRepository;
    private readonly DocumentNumberService _documentNumbers;
    private readonly IProductRepository _productRepository;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IProductStockRepository _productStockRepository;
    private readonly IStockTransactionRepository _stockTransactionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IStorageLocationRepository _storageLocationRepository;
    private readonly IProductLocationStockRepository _productLocationStockRepository;
    private readonly StockMovementService _stockMovementService;
    private readonly InventoryReservationService _inventoryReservationService;
    private readonly InventoryTrackingService _inventoryTrackingService;
    private readonly IInventoryAccessRepository _inventoryAccessRepository;
    private readonly IBranchContext _branchContext;

    public StockTransferService(
        IStockTransferRepository stockTransferRepository,
        DocumentNumberService documentNumbers,
        IProductRepository productRepository,
        IWarehouseRepository warehouseRepository,
        IProductStockRepository productStockRepository,
        IStockTransactionRepository stockTransactionRepository,
        IUnitOfWork unitOfWork,
        IStorageLocationRepository storageLocationRepository,
        IProductLocationStockRepository productLocationStockRepository,
        StockMovementService stockMovementService,
        InventoryReservationService inventoryReservationService,
        InventoryTrackingService inventoryTrackingService,
        IInventoryAccessRepository inventoryAccessRepository,
        IBranchContext branchContext)
    {
        _stockTransferRepository = stockTransferRepository;
        _documentNumbers = documentNumbers;
        _productRepository = productRepository;
        _warehouseRepository = warehouseRepository;
        _productStockRepository = productStockRepository;
        _stockTransactionRepository = stockTransactionRepository;
        _unitOfWork = unitOfWork;
        _storageLocationRepository = storageLocationRepository;
        _productLocationStockRepository = productLocationStockRepository;
        _stockMovementService = stockMovementService;
        _inventoryReservationService = inventoryReservationService;
        _inventoryTrackingService = inventoryTrackingService;
        _inventoryAccessRepository = inventoryAccessRepository;
        _branchContext = branchContext;
    }

    public async Task<List<StockTransfer>> GetAllAsync(
        string? search,
        StockTransferStatus? status)
    {
        return await _stockTransferRepository.GetAllAsync(
            search,
            status);
    }

    public async Task<StockTransfer?> GetByIdAsync(
        int id)
    {
        return await _stockTransferRepository.GetByIdAsync(id);
    }

    public async Task<int> CreateAsync(
        CreateStockTransferCommand command)
    {
        if (command == null)
            throw new ArgumentNullException(nameof(command));

        if (string.IsNullOrWhiteSpace(command.CreatedByUserId))
            throw new ArgumentException(
                "The user who created the transfer is required.");

        if (!command.Items.Any())
            throw new InvalidOperationException(
                "A transfer must contain at least one item.");

        var fromWarehouse =
            await _warehouseRepository.GetByIdAsync(
                command.FromWarehouseId);

        if (fromWarehouse == null)
            throw new ResourceNotFoundException("Source warehouse not found.");

        var toWarehouse =
            await _warehouseRepository.GetByIdAsync(
                command.ToWarehouseId);

        if (toWarehouse == null)
            throw new ResourceNotFoundException("Destination warehouse not found.");

        if (command.FromWarehouseId == command.ToWarehouseId)
            throw new InvalidOperationException(
                "Source and destination warehouses must be different.");

        var duplicateProductIds = command.Items
            .GroupBy(x => x.ProductId)
            .Where(x => x.Count() > 1)
            .Select(x => x.Key)
            .ToList();

        if (duplicateProductIds.Any())
            throw new InvalidOperationException(
                "The same product cannot be added more than once.");

        foreach (var item in command.Items)
        {
            if (item.ProductId <= 0)
                throw new ArgumentException(
                    "Product is required.");

            if (item.Quantity <= 0)
                throw new ArgumentException(
                    "Transfer quantity must be greater than zero.");

            var product =
                await _productRepository.GetByIdAsync(
                    item.ProductId);

            if (product == null)
            {
                throw new ResourceNotFoundException($"Product with ID {item.ProductId} was not found.");
            }

            await ValidateLocationsAsync(
                item.SourceLocationId,
                item.DestinationLocationId,
                fromWarehouse,
                toWarehouse);
        }

        StockTransfer? transfer = null;

        await _unitOfWork.ExecuteInTransactionAsync(
            async () =>
            {
                if (command.Reference?.StartsWith("REPLENISHMENT:RULE:", StringComparison.Ordinal) == true)
                {
                    var existing = await _stockTransferRepository.GetAllAsync(command.Reference, null);
                    if (existing.Any(x => x.Reference == command.Reference &&
                        x.Status is StockTransferStatus.Draft or StockTransferStatus.Submitted or StockTransferStatus.Approved))
                        throw new DuplicateResourceException("An open transfer already exists for this replenishment rule.");
                }

                transfer = new StockTransfer(
                    await _documentNumbers.GenerateAsync(
                        DocumentNumberType.StockTransfer,
                        DateTime.Today),
                    command.FromWarehouseId,
                    command.ToWarehouseId,
                    command.CreatedByUserId,
                    command.Reference,
                    command.Notes);

                foreach (var item in command.Items)
                {
                    transfer.AddItem(
                        new StockTransferItem(
                            item.ProductId,
                            item.Quantity,
                            item.SourceLocationId,
                            item.DestinationLocationId,
                            item.TrackingAllocations));
                }

                await _stockTransferRepository.AddAsync(
                    transfer);
            });

        return transfer!.Id;
    }

    public async Task UpdateAsync(
       int id,
       UpdateStockTransferCommand command)
    {
        if (command == null)
            throw new ArgumentNullException(nameof(command));

        if (command.Id != id)
            throw new InvalidOperationException(
                "Transfer ID mismatch.");

        var transfer =
            await _stockTransferRepository.GetByIdAsync(id);

        if (transfer == null)
            throw new ResourceNotFoundException("Stock transfer not found.");

        if (!command.Items.Any())
            throw new InvalidOperationException(
                "A transfer must contain at least one item.");

        if (command.FromWarehouseId <= 0)
            throw new ArgumentException(
                "Source warehouse is required.");

        if (command.ToWarehouseId <= 0)
            throw new ArgumentException(
                "Destination warehouse is required.");

        if (command.FromWarehouseId ==
            command.ToWarehouseId)
        {
            throw new InvalidOperationException(
                "Source and destination warehouses must be different.");
        }

        var fromWarehouse =
            await _warehouseRepository.GetByIdAsync(
                command.FromWarehouseId);

        if (fromWarehouse == null)
            throw new ResourceNotFoundException("Source warehouse not found.");

        var toWarehouse =
            await _warehouseRepository.GetByIdAsync(
                command.ToWarehouseId);

        if (toWarehouse == null)
            throw new ResourceNotFoundException("Destination warehouse not found.");

        var duplicateProductIds =
            command.Items
                .GroupBy(x => x.ProductId)
                .Where(x => x.Count() > 1)
                .Select(x => x.Key)
                .ToList();

        if (duplicateProductIds.Any())
            throw new InvalidOperationException(
                "The same product cannot be added more than once.");

        foreach (var item in command.Items)
        {
            if (item.ProductId <= 0)
                throw new ArgumentException(
                    "Product is required.");

            if (item.Quantity <= 0)
                throw new ArgumentException(
                    "Transfer quantity must be greater than zero.");

            var product =
                await _productRepository.GetByIdAsync(
                    item.ProductId);

            if (product == null)
            {
                throw new ResourceNotFoundException($"Product with ID {item.ProductId} was not found.");
            }

            await ValidateLocationsAsync(
                item.SourceLocationId,
                item.DestinationLocationId,
                fromWarehouse,
                toWarehouse);
        }

        await _unitOfWork.ExecuteInTransactionAsync(
            async () =>
            {
                // Change header information.
                transfer.ChangeWarehouses(
                    command.FromWarehouseId,
                    command.ToWarehouseId);

                transfer.SetReference(
                    command.Reference);

                transfer.SetNotes(
                    command.Notes);

                // Remove items that no longer exist
                // in the edited transfer.
                var requestedProductIds =
                    command.Items
                        .Select(x => x.ProductId)
                        .ToHashSet();

                var existingItemsToRemove =
                    transfer.Items
                        .Where(x =>
                            !requestedProductIds.Contains(
                                x.ProductId))
                        .ToList();

                foreach (var existingItem in existingItemsToRemove)
                {
                    transfer.RemoveItem(
                        existingItem.ProductId);

                    await _stockTransferRepository
                        .DeleteItemAsync(
                            existingItem);
                }

                // Add new items and update existing ones.
                foreach (var requestedItem in command.Items)
                {
                    var existingItem =
                        transfer.Items.FirstOrDefault(
                            x =>
                                x.ProductId ==
                                requestedItem.ProductId);

                    if (existingItem == null)
                    {
                        transfer.AddItem(
                            new StockTransferItem(
                                requestedItem.ProductId,
                                requestedItem.Quantity,
                                requestedItem.SourceLocationId,
                                requestedItem.DestinationLocationId,
                                requestedItem.TrackingAllocations));
                    }
                    else
                    {
                        transfer.ChangeItemQuantity(
                            requestedItem.ProductId,
                            requestedItem.Quantity);
                        existingItem.ChangeLocations(requestedItem.SourceLocationId, requestedItem.DestinationLocationId);
                        existingItem.ChangeTrackingAllocations(requestedItem.TrackingAllocations);
                    }
                }
            });
    }

    private async Task ValidateLocationsAsync(
        int? sourceLocationId,
        int? destinationLocationId,
        Warehouse fromWarehouse,
        Warehouse toWarehouse)
    {
        if (fromWarehouse.ControlMode == InventoryControlMode.Simple &&
            sourceLocationId.HasValue)
        {
            throw new InvalidOperationException(
                "A source location cannot be selected for a simple warehouse.");
        }

        if (toWarehouse.ControlMode == InventoryControlMode.Simple &&
            destinationLocationId.HasValue)
        {
            throw new InvalidOperationException(
                "A destination location cannot be selected for a simple warehouse.");
        }

        if (fromWarehouse.RequireSourceLocationForTransfers &&
            !sourceLocationId.HasValue)
        {
            throw new InvalidOperationException(
                "The source warehouse requires an exact source location.");
        }

        if (toWarehouse.RequireDestinationLocationForTransfers &&
            !destinationLocationId.HasValue)
        {
            throw new InvalidOperationException(
                "The destination warehouse requires an exact destination location.");
        }

        if (sourceLocationId.HasValue)
        {
            var source = await _storageLocationRepository
                .GetByIdAsync(sourceLocationId.Value);

            if (source is null || source.WarehouseId != fromWarehouse.Id)
            {
                throw new InvalidOperationException(
                    "Source location must belong to the source warehouse.");
            }

            if (source.Status != StorageLocationStatus.Active)
            {
                throw new InvalidOperationException("Source location must be active.");
            }

            if (!source.IsPickable)
            {
                throw new InvalidOperationException(
                    "Source location does not allow picking.");
            }
        }

        if (destinationLocationId.HasValue)
        {
            var destination = await _storageLocationRepository
                .GetByIdAsync(destinationLocationId.Value);

            if (destination is null || destination.WarehouseId != toWarehouse.Id)
            {
                throw new InvalidOperationException(
                    "Destination location must belong to the destination warehouse.");
            }

            if (destination.Status != StorageLocationStatus.Active)
            {
                throw new InvalidOperationException(
                    "Destination location must be active.");
            }

            if (!destination.IsReceivable)
            {
                throw new InvalidOperationException(
                    "Destination location does not allow receiving.");
            }
        }
    }


    public async Task SubmitAsync(
        int id,
        string userId)
    {
        var transfer =
            await _stockTransferRepository.GetByIdAsync(id);

        if (transfer == null)
            throw new ResourceNotFoundException("Stock transfer not found.");

        var fromWarehouse = await _warehouseRepository.GetByIdAsync(transfer.FromWarehouseId)
            ?? throw new ResourceNotFoundException("Source warehouse not found.");
        var toWarehouse = await _warehouseRepository.GetByIdAsync(transfer.ToWarehouseId)
            ?? throw new ResourceNotFoundException("Destination warehouse not found.");

        foreach (var item in transfer.Items)
        {
            await ValidateLocationsAsync(
                item.SourceLocationId,
                item.DestinationLocationId,
                fromWarehouse,
                toWarehouse);
        }

        await _unitOfWork.ExecuteInTransactionAsync(
            async () =>
            {
                transfer.Submit(userId);
            });
    }

    public async Task ApproveAsync(
        int id,
        string userId)
    {
        var transfer =
            await _stockTransferRepository.GetByIdAsync(id);

        if (transfer == null)
            throw new ResourceNotFoundException("Stock transfer not found.");

        await _unitOfWork.ExecuteInTransactionAsync(
            async () =>
            {
                transfer.Approve(userId);
                await _inventoryReservationService.ReserveTransferAsync(transfer, userId);
                await _stockMovementService.PlanTransferAsync(transfer, userId);
            });
    }

    public async Task RejectAsync(
        int id,
        string userId,
        string reason)
    {
        var transfer =
            await _stockTransferRepository.GetByIdAsync(id);

        if (transfer == null)
            throw new ResourceNotFoundException("Stock transfer not found.");

        await _unitOfWork.ExecuteInTransactionAsync(
            async () =>
            {
                transfer.Reject(
                    userId,
                    reason);
            });
    }

    public async Task ReturnToDraftAsync(
        int id,
        string userId)
    {
        var transfer =
            await _stockTransferRepository.GetByIdAsync(id);

        if (transfer == null)
            throw new ResourceNotFoundException("Stock transfer not found.");

        await _unitOfWork.ExecuteInTransactionAsync(
            async () =>
            {
                transfer.ReturnToDraft(userId);
            });
    }

    public async Task PostAsync(
        int id,
        string userId)
    {
        var transfer = await _stockTransferRepository.GetByIdAsync(id)
            ?? throw new ResourceNotFoundException("Stock transfer not found.");
        var source = await _warehouseRepository.GetByIdAsync(transfer.FromWarehouseId)
            ?? throw new ResourceNotFoundException("Source warehouse not found.");
        var destination = await _warehouseRepository.GetByIdAsync(transfer.ToWarehouseId)
            ?? throw new ResourceNotFoundException("Destination warehouse not found.");
        if (!source.BranchId.HasValue || !destination.BranchId.HasValue ||
            source.BranchId == destination.BranchId)
            throw new BusinessConflictException("Dispatch requires two different assigned branches.");
        await RequireBranchOperationAsync(source, transferOut: true);
        await _unitOfWork.ExecuteInTransactionAsync(() =>
        {
            transfer.Dispatch(userId);
            return Task.CompletedTask;
        });
    }

    public async Task ReceiveAsync(
        int id,
        string userId,
        IReadOnlyList<ReceiveStockTransferLineCommand> lines)
    {
        var transfer = await _stockTransferRepository.GetByIdAsync(id)
            ?? throw new ResourceNotFoundException("Stock transfer not found.");
        if (lines.Count == 0 || lines.GroupBy(x => x.StockTransferItemId).Any(x => x.Count() > 1))
            throw new ArgumentException("Receipt lines must be unique and cannot be empty.");
        var sourceWarehouse = await _warehouseRepository.GetByIdAsync(transfer.FromWarehouseId)
            ?? throw new ResourceNotFoundException("Source warehouse not found.");
        var destinationWarehouse = await _warehouseRepository.GetByIdAsync(transfer.ToWarehouseId)
            ?? throw new ResourceNotFoundException("Destination warehouse not found.");
        await RequireBranchOperationAsync(destinationWarehouse, transferOut: false);

        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            foreach (var line in lines)
            {
                var item = transfer.Items.SingleOrDefault(x => x.Id == line.StockTransferItemId)
                    ?? throw new InvalidOperationException("Receipt line does not belong to this transfer.");
                var destinationLocationId = line.DestinationLocationId ?? item.DestinationLocationId;
                await ValidateLocationsAsync(item.SourceLocationId, destinationLocationId,
                    sourceWarehouse, destinationWarehouse);
                if (line.ReceivedQuantity > 0)
                {
                    await MoveReceivedQuantityAsync(
                        transfer, item, line.ReceivedQuantity, destinationLocationId,
                        line.TrackingAllocations, userId, destinationWarehouse);
                }
                item.RecordReceipt(line.ReceivedQuantity, line.ShortQuantity, line.DamagedQuantity);
            }
            transfer.RecordReceipt(userId);
            if (transfer.Status == StockTransferStatus.Received)
                await _stockMovementService.PostTransferAsync(transfer, userId);
        });
    }

    private async Task MoveReceivedQuantityAsync(
        StockTransfer transfer,
        StockTransferItem item,
        decimal quantity,
        int? destinationLocationId,
        string? trackingAllocations,
        string userId,
        Warehouse destinationWarehouse)
    {
        var product = await _productRepository.GetByIdAsync(item.ProductId)
            ?? throw new ResourceNotFoundException("Transfer product was not found.");
        var sourceStock = await _productStockRepository.GetByProductAndWarehouseAsync(
            item.ProductId, transfer.FromWarehouseId)
            ?? throw new BusinessConflictException("Source stock record was not found.");
        if (sourceStock.Quantity < quantity)
            throw new BusinessConflictException("Insufficient source stock for this receipt.");
        if (item.SourceLocationId.HasValue)
        {
            var sourceLocationStock = await _productLocationStockRepository.GetAsync(
                item.ProductId, item.SourceLocationId.Value)
                ?? throw new BusinessConflictException("Source location stock was not found.");
            sourceLocationStock.RemoveQuantity(quantity);
        }
        else
        {
            var assigned = (await _productLocationStockRepository.GetAllAsync())
                .Where(x => x.ProductId == item.ProductId && x.WarehouseId == transfer.FromWarehouseId)
                .Sum(x => x.Quantity);
            if (sourceStock.Quantity - assigned < quantity)
                throw new BusinessConflictException("Insufficient unassigned source stock for this receipt.");
        }

        await _inventoryReservationService.ConsumeTransferAsync(
            transfer.Id, new Dictionary<int, decimal> { [item.Id] = quantity }, userId);
        var destinationStock = await _productStockRepository.GetByProductAndWarehouseAsync(
            item.ProductId, transfer.ToWarehouseId);
        if (destinationStock is null)
        {
            destinationStock = new ProductStock(item.ProductId, transfer.ToWarehouseId);
            await _productStockRepository.AddAsync(destinationStock);
        }
        var sourceCost = sourceStock.RemoveQuantity(quantity);
        var destinationCost = destinationStock.Receive(quantity, sourceCost.UnitCost);

        if (destinationLocationId.HasValue)
        {
            var location = await _storageLocationRepository.GetByIdAsync(destinationLocationId.Value)
                ?? throw new ResourceNotFoundException("Destination location was not found.");
            var locationStock = await _productLocationStockRepository.GetAsync(
                item.ProductId, destinationLocationId.Value);
            if (locationStock is null)
            {
                locationStock = new ProductLocationStock(
                    item.ProductId, transfer.ToWarehouseId, destinationLocationId.Value);
                await _productLocationStockRepository.AddAsync(locationStock);
            }
            if (destinationWarehouse.EnforceLocationCapacity && location.MaximumQuantity.HasValue)
            {
                var current = (await _productLocationStockRepository.GetAllAsync())
                    .Where(x => x.StorageLocationId == location.Id).Sum(x => x.Quantity);
                if (current + quantity > location.MaximumQuantity.Value)
                    throw new BusinessConflictException("Receipt quantity exceeds destination location capacity.");
            }
            locationStock.AddQuantity(quantity);
        }

        await _stockTransactionRepository.AddAsync(new StockTransaction(
            item.ProductId, transfer.FromWarehouseId, -quantity,
            StockTransactionType.TransferOut, transfer.TransferNumber, sourceCost));
        await _stockTransactionRepository.AddAsync(new StockTransaction(
            item.ProductId, transfer.ToWarehouseId, quantity,
            StockTransactionType.TransferIn, transfer.TransferNumber, destinationCost));
        await _inventoryTrackingService.TransferAsync(
            product, transfer.FromWarehouseId, transfer.ToWarehouseId,
            item.SourceLocationId, destinationLocationId, quantity,
            transfer.TransferNumber, userId, trackingAllocations);
    }

    private async Task RequireBranchOperationAsync(Warehouse warehouse, bool transferOut)
    {
        if (!warehouse.BranchId.HasValue)
            throw new BusinessConflictException("Inter-branch transfer warehouses must belong to branches.");
        if (_branchContext.BranchId != warehouse.BranchId)
            throw new BusinessConflictException(transferOut
                ? "Select the source branch before dispatching this transfer."
                : "Select the destination branch before receiving this transfer.");
        var access = await _inventoryAccessRepository.GetBranchAccessAsync(
            warehouse.BranchId.Value, warehouse.Id);
        if (access is null || (transferOut ? !access.AllowTransferOut : !access.AllowTransferIn))
            throw new BusinessConflictException(transferOut
                ? "The source warehouse does not allow transfer dispatch."
                : "The destination warehouse does not allow transfer receipt.");
    }

    public async Task CancelAsync(
        int id,
        string userId,
        string reason)
    {
        var transfer =
            await _stockTransferRepository.GetByIdAsync(id);

        if (transfer == null)
            throw new ResourceNotFoundException("Stock transfer not found.");

        await _unitOfWork.ExecuteInTransactionAsync(
            async () =>
            {
                foreach (var item in transfer.Items)
                {
                    var trackedProduct = await _productRepository.GetByIdAsync(item.ProductId)
                        ?? throw new ResourceNotFoundException("Transfer product was not found.");
                    var sourceStock =
                        await _productStockRepository
                            .GetByProductAndWarehouseAsync(
                                item.ProductId,
                                transfer.FromWarehouseId);

                    if (sourceStock == null)
                    {
                        throw new BusinessConflictException($"No stock record exists for product ID {item.ProductId} in the source warehouse.");
                    }

                    var destinationStock =
                        await _productStockRepository
                            .GetByProductAndWarehouseAsync(
                                item.ProductId,
                                transfer.ToWarehouseId);

                    if (destinationStock == null)
                    {
                        throw new BusinessConflictException($"No stock record exists for product ID {item.ProductId} in the destination warehouse.");
                    }

                    if (destinationStock.Quantity < item.Quantity)
                    {
                        throw new BusinessConflictException($"Insufficient stock in the destination warehouse to reverse product ID {item.ProductId}.");
                    }

                    if (item.DestinationLocationId.HasValue)
                    {
                        var destinationLocationStock =
                            await _productLocationStockRepository.GetAsync(
                                item.ProductId,
                                item.DestinationLocationId.Value)
                            ?? throw new BusinessConflictException("Destination location stock is missing and cannot be reversed.");

                        destinationLocationStock.RemoveQuantity(item.Quantity);
                    }
                    else
                    {
                        var locationStocks =
                            await _productLocationStockRepository.GetAllAsync();
                        var assignedQuantity = locationStocks
                            .Where(stock =>
                                stock.ProductId == item.ProductId &&
                                stock.WarehouseId == transfer.ToWarehouseId)
                            .Sum(stock => stock.Quantity);

                        if (destinationStock.Quantity - assignedQuantity < item.Quantity)
                        {
                            throw new BusinessConflictException("Insufficient unassigned destination stock to reverse the transfer.");
                        }
                    }

                    var destinationCostMovement = destinationStock.RemoveQuantity(item.Quantity);
                    var sourceCostMovement = sourceStock.Receive(
                        item.Quantity,
                        destinationCostMovement.UnitCost);

                    if (item.SourceLocationId.HasValue)
                    {
                        var sourceLocationStock =
                            await _productLocationStockRepository.GetAsync(
                                item.ProductId,
                                item.SourceLocationId.Value);

                        if (sourceLocationStock is null)
                        {
                            sourceLocationStock = new ProductLocationStock(
                                item.ProductId,
                                transfer.FromWarehouseId,
                                item.SourceLocationId.Value);

                            await _productLocationStockRepository.AddAsync(
                                sourceLocationStock);
                        }

                        sourceLocationStock.AddQuantity(item.Quantity);
                    }

                    await _stockTransactionRepository.AddAsync(
                        new StockTransaction(
                            item.ProductId,
                            transfer.FromWarehouseId,
                            item.Quantity,
                            StockTransactionType.TransferIn,
                            $"Cancellation of {transfer.TransferNumber}",
                            sourceCostMovement));

                    await _stockTransactionRepository.AddAsync(
                        new StockTransaction(
                            item.ProductId,
                            transfer.ToWarehouseId,
                            -item.Quantity,
                            StockTransactionType.TransferOut,
                            $"Cancellation of {transfer.TransferNumber}",
                            destinationCostMovement));

                    var cancellationAllocations = await _inventoryTrackingService.GetTransferredAllocationTextAsync(
                        transfer.TransferNumber, item.ProductId, transfer.ToWarehouseId, item.DestinationLocationId);
                    await _inventoryTrackingService.TransferAsync(trackedProduct,
                        transfer.ToWarehouseId, transfer.FromWarehouseId,
                        item.DestinationLocationId, item.SourceLocationId,
                        item.Quantity, $"Cancellation of {transfer.TransferNumber}", userId, cancellationAllocations);
                }

                await _stockMovementService.ReverseTransferAsync(transfer.Id, userId);
                transfer.Cancel(
                    userId,
                    reason);
            });
    }
}
