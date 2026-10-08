using MiniStore.Domain.Entities;

namespace MiniStore.Domain.Interfaces;

public interface IPurchaseSourcingRepository
{
    Task<List<PurchaseSourcingEvent>> GetAllAsync(PurchaseSourcingStatus? status, string? search);
    Task<PurchaseSourcingEvent?> GetByIdAsync(int id);
    Task<PurchaseSourcingEvent?> GetByPurchaseRequestIdAsync(int purchaseRequestId);
    Task AddAsync(PurchaseSourcingEvent sourcingEvent);
}
