using MiniStore.Domain.Entities;

namespace MiniStore.Domain.Interfaces;

public interface IPurchaseSourcingRepository
{
    Task<List<PurchaseSourcingEvent>> GetAllAsync(PurchaseSourcingStatus? status, string? search);
    Task<PurchaseSourcingEvent?> GetByIdAsync(int id);
    Task<PurchaseSourcingEvent?> GetByPurchaseRequestIdAsync(int purchaseRequestId);
    Task<PurchaseSourcingAttachment?> GetAttachmentAsync(int sourcingEventId, int attachmentId);
    Task AddAsync(PurchaseSourcingEvent sourcingEvent);
}

public interface IPurchaseSourcingPolicyRepository
{
    Task<PurchaseSourcingPolicy?> GetAsync();
    Task AddAsync(PurchaseSourcingPolicy policy);
    Task SaveChangesAsync();
}
