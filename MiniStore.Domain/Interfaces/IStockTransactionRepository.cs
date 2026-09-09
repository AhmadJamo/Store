using MiniStore.Domain.Entities;

namespace MiniStore.Domain.Interfaces;

public interface IStockTransactionRepository
{
    Task<List<StockTransaction>> GetAllAsync();

    Task<List<StockTransaction>> GetByProductAndWarehouseAsync(
        int productId,
        int warehouseId);

    Task AddAsync(StockTransaction transaction);

    Task SaveChangesAsync();
}