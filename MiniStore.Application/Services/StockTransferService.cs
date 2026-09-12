using MiniStore.Domain.Commands;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public class StockTransferService : IStockTransferService
{
    private readonly IStockTransferRepository _stockTransferRepository;
    private readonly IDocumentNumberSettingsRepository _documentNumberSettingsRepository;
    private readonly IProductRepository _productRepository;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IProductStockRepository _productStockRepository;
    private readonly IStockTransactionRepository _stockTransactionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public StockTransferService(
        IStockTransferRepository stockTransferRepository,
        IDocumentNumberSettingsRepository documentNumberSettingsRepository,
        IProductRepository productRepository,
        IWarehouseRepository warehouseRepository,
        IProductStockRepository productStockRepository,
        IStockTransactionRepository stockTransactionRepository,
        IUnitOfWork unitOfWork)
    {
        _stockTransferRepository = stockTransferRepository;
        _documentNumberSettingsRepository =
            documentNumberSettingsRepository;
        _productRepository = productRepository;
        _warehouseRepository = warehouseRepository;
        _productStockRepository = productStockRepository;
        _stockTransactionRepository = stockTransactionRepository;
        _unitOfWork = unitOfWork;
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
            throw new InvalidOperationException(
                "Source warehouse not found.");

        var toWarehouse =
            await _warehouseRepository.GetByIdAsync(
                command.ToWarehouseId);

        if (toWarehouse == null)
            throw new InvalidOperationException(
                "Destination warehouse not found.");

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
                throw new InvalidOperationException(
                    $"Product with ID {item.ProductId} was not found.");
            }
        }

        var documentSettings =
            await _documentNumberSettingsRepository.GetAsync();

        if (documentSettings == null)
            throw new InvalidOperationException(
                "Document number settings were not found.");

        var transferNumber =
            documentSettings.GenerateNextStockTransferNumber();

        var transfer = new StockTransfer(
            transferNumber,
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
                    item.Quantity));
        }

        await _unitOfWork.ExecuteInTransactionAsync(
            async () =>
            {
                await _stockTransferRepository.AddAsync(
                    transfer);
            });

        return transfer.Id;
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
            throw new InvalidOperationException(
                "Stock transfer not found.");

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
            throw new InvalidOperationException(
                "Source warehouse not found.");

        var toWarehouse =
            await _warehouseRepository.GetByIdAsync(
                command.ToWarehouseId);

        if (toWarehouse == null)
            throw new InvalidOperationException(
                "Destination warehouse not found.");

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
                throw new InvalidOperationException(
                    $"Product with ID {item.ProductId} was not found.");
            }
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
                                requestedItem.Quantity));
                    }
                    else
                    {
                        transfer.ChangeItemQuantity(
                            requestedItem.ProductId,
                            requestedItem.Quantity);
                    }
                }
            });
    }


    public async Task SubmitAsync(
        int id,
        string userId)
    {
        var transfer =
            await _stockTransferRepository.GetByIdAsync(id);

        if (transfer == null)
            throw new InvalidOperationException(
                "Stock transfer not found.");

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
            throw new InvalidOperationException(
                "Stock transfer not found.");

        await _unitOfWork.ExecuteInTransactionAsync(
            async () =>
            {
                transfer.Approve(userId);
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
            throw new InvalidOperationException(
                "Stock transfer not found.");

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
            throw new InvalidOperationException(
                "Stock transfer not found.");

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
        var transfer =
            await _stockTransferRepository.GetByIdAsync(id);

        if (transfer == null)
            throw new InvalidOperationException(
                "Stock transfer not found.");

        await _unitOfWork.ExecuteInTransactionAsync(
            async () =>
            {
                foreach (var item in transfer.Items)
                {
                    var sourceStock =
                        await _productStockRepository
                            .GetByProductAndWarehouseAsync(
                                item.ProductId,
                                transfer.FromWarehouseId);

                    if (sourceStock == null)
                    {
                        throw new InvalidOperationException(
                            $"No stock record exists for product ID {item.ProductId} in the source warehouse.");
                    }

                    if (sourceStock.Quantity < item.Quantity)
                    {
                        throw new InvalidOperationException(
                            $"Insufficient stock for product ID {item.ProductId}.");
                    }

                    var destinationStock =
                        await _productStockRepository
                            .GetByProductAndWarehouseAsync(
                                item.ProductId,
                                transfer.ToWarehouseId);

                    if (destinationStock == null)
                    {
                        destinationStock =
                            new ProductStock(
                                item.ProductId,
                                transfer.ToWarehouseId);

                        await _productStockRepository.AddAsync(
                            destinationStock);
                    }

                    sourceStock.RemoveQuantity(
                        item.Quantity);

                    destinationStock.AddQuantity(
                        item.Quantity);

                    await _stockTransactionRepository.AddAsync(
                        new StockTransaction(
                            item.ProductId,
                            transfer.FromWarehouseId,
                            -item.Quantity,
                            StockTransactionType.TransferOut,
                            transfer.TransferNumber));

                    await _stockTransactionRepository.AddAsync(
                        new StockTransaction(
                            item.ProductId,
                            transfer.ToWarehouseId,
                            item.Quantity,
                            StockTransactionType.TransferIn,
                            transfer.TransferNumber));
                }

                transfer.Post(userId);
            });
    }

    public async Task CancelAsync(
        int id,
        string userId,
        string reason)
    {
        var transfer =
            await _stockTransferRepository.GetByIdAsync(id);

        if (transfer == null)
            throw new InvalidOperationException(
                "Stock transfer not found.");

        await _unitOfWork.ExecuteInTransactionAsync(
            async () =>
            {
                foreach (var item in transfer.Items)
                {
                    var sourceStock =
                        await _productStockRepository
                            .GetByProductAndWarehouseAsync(
                                item.ProductId,
                                transfer.FromWarehouseId);

                    if (sourceStock == null)
                    {
                        throw new InvalidOperationException(
                            $"No stock record exists for product ID {item.ProductId} in the source warehouse.");
                    }

                    var destinationStock =
                        await _productStockRepository
                            .GetByProductAndWarehouseAsync(
                                item.ProductId,
                                transfer.ToWarehouseId);

                    if (destinationStock == null)
                    {
                        throw new InvalidOperationException(
                            $"No stock record exists for product ID {item.ProductId} in the destination warehouse.");
                    }

                    if (destinationStock.Quantity < item.Quantity)
                    {
                        throw new InvalidOperationException(
                            $"Insufficient stock in the destination warehouse to reverse product ID {item.ProductId}.");
                    }

                    destinationStock.RemoveQuantity(
                        item.Quantity);

                    sourceStock.AddQuantity(
                        item.Quantity);

                    await _stockTransactionRepository.AddAsync(
                        new StockTransaction(
                            item.ProductId,
                            transfer.FromWarehouseId,
                            item.Quantity,
                            StockTransactionType.TransferIn,
                            $"Cancellation of {transfer.TransferNumber}"));

                    await _stockTransactionRepository.AddAsync(
                        new StockTransaction(
                            item.ProductId,
                            transfer.ToWarehouseId,
                            -item.Quantity,
                            StockTransactionType.TransferOut,
                            $"Cancellation of {transfer.TransferNumber}"));
                }

                transfer.Cancel(
                    userId,
                    reason);
            });
    }
}