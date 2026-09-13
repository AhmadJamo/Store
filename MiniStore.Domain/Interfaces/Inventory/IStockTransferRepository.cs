using MiniStore.Domain.Entities;

namespace MiniStore.Domain.Interfaces;

public interface IStockTransferRepository
{
    Task<StockTransfer?> GetByIdAsync(
        int id);

    Task<StockTransfer?> GetByTransferNumberAsync(
        string transferNumber);

    Task<List<StockTransfer>> GetAllAsync(
        string? search,
        StockTransferStatus? status);

    Task AddAsync(
        StockTransfer transfer);

    Task DeleteItemAsync(
    StockTransferItem item);

    Task SaveChangesAsync();
}