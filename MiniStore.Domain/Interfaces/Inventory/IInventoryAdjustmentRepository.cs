using MiniStore.Domain.Entities;
namespace MiniStore.Domain.Interfaces;
public interface IInventoryAdjustmentRepository
{
    Task<List<InventoryAdjustment>> GetAllAsync();
    Task<InventoryAdjustment?> GetByIdAsync(long id);
    Task<InventoryBalance?> GetBalanceAsync(int productId, int warehouseId, int? locationId);
    Task AddAsync(InventoryAdjustment adjustment);
}
