using MiniStore.Domain.Entities;
namespace MiniStore.Domain.Interfaces;
public interface IPurchaseRequestRepository
{
    Task<List<PurchaseRequest>> GetAllAsync(PurchaseRequestStatus? status, string? search);
    Task<PurchaseRequest?> GetByIdAsync(int id);
    Task AddAsync(PurchaseRequest request);
    Task SaveChangesAsync();
}
