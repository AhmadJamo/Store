using MiniStore.Domain.Entities;
namespace MiniStore.Domain.Interfaces;
public interface IInventoryTrackingRepository
{
    Task<List<InventoryTrackingBalance>> GetBalancesAsync();
    Task<List<InventoryTrackingTransaction>> GetTransactionsAsync();
    Task<bool> IdentifierExistsAsync(int productId, ProductTrackingPolicy policy, string identifier);
    Task AddBalanceAsync(InventoryTrackingBalance balance);
    Task AddTransactionAsync(InventoryTrackingTransaction transaction);
}
