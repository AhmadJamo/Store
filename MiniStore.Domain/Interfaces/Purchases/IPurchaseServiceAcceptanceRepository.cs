using MiniStore.Domain.Entities;

namespace MiniStore.Domain.Interfaces;

public interface IPurchaseServiceAcceptanceRepository
{
    Task<List<PurchaseServiceAcceptance>> GetAllAsync();
    Task<List<PurchaseServiceAcceptance>> GetByPurchaseOrderIdAsync(int purchaseOrderId);
    Task<PurchaseServiceAcceptance?> GetByIdAsync(int id);
    Task AddAsync(PurchaseServiceAcceptance acceptance);
}
