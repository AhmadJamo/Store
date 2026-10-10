using MiniStore.Domain.Entities;
namespace MiniStore.Domain.Interfaces;
public interface IPurchaseRequestRepository
{
    Task<List<PurchaseRequest>> GetAllAsync(PurchaseRequestStatus? status, string? search);
    Task<PurchaseRequest?> GetByIdAsync(int id);
    Task<PurchaseRequest?> GetActiveBySourceAsync(string sourceType, string sourceReference);
    Task<List<PurchaseRequest>> GetActiveBySourceTypeAsync(string sourceType);
    Task AddAsync(PurchaseRequest request);
    Task SaveChangesAsync();
}
