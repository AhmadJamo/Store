using MiniStore.Domain.Entities;

namespace MiniStore.Domain.Interfaces;

public interface IInventoryBalanceRepository
{
    Task<List<InventoryBalance>> GetAllAsync();
    Task<decimal> GetWarehouseAvailableAsync(int productId, int warehouseId);
}
