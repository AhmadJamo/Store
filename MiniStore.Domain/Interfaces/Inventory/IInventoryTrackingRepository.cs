using MiniStore.Domain.Entities;
namespace MiniStore.Domain.Interfaces;
public interface IInventoryTrackingRepository
{
    Task<List<InventoryTrackingBalance>> GetBalancesAsync();
    Task<List<InventoryTrackingTransaction>> GetTransactionsAsync();
    Task<bool> IdentifierExistsAsync(int productId, ProductTrackingPolicy policy, string identifier);
    Task<InventoryTrackingBalance?> GetBalanceAsync(int productId, int warehouseId, int? locationId, string identifier);
    Task<List<InventoryTrackingBalance>> GetAvailableForUpdateAsync(int productId, int warehouseId, int? locationId = null);
    Task<List<InventoryTrackingBalance>> GetByIdentifierForUpdateAsync(int productId, string identifier);
    Task AddBalanceAsync(InventoryTrackingBalance balance);
    Task AddTransactionAsync(InventoryTrackingTransaction transaction);
}
