using MiniStore.Domain.Entities;
namespace MiniStore.Domain.Interfaces;
public interface IPurchaseReturnRepository
{
    Task<List<PurchaseReturn>> GetAllAsync();
    Task<PurchaseReturn?> GetByIdAsync(int id);
    Task<List<PurchaseReturn>> GetByPurchaseIdAsync(int purchaseId);
    Task AddAsync(PurchaseReturn purchaseReturn);
}
