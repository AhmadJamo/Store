using MiniStore.Domain.Entities;

namespace MiniStore.Domain.Interfaces;

public interface IStockTransactionRepository
{
    Task<List<StockTransaction>> GetAllAsync();

    Task<(List<StockTransaction> Items, int TotalCount)> GetPagedAsync(
        string? search,
        StockTransactionType? type,
        bool? incoming,
        DateTime? fromDate,
        DateTime? toDate,
        int page,
        int pageSize,
        string sortColumn,
        bool sortDescending);

    Task<List<StockTransaction>> GetByProductAndWarehouseAsync(
        int productId,
        int warehouseId);

    Task AddAsync(StockTransaction transaction);

    Task SaveChangesAsync();
}