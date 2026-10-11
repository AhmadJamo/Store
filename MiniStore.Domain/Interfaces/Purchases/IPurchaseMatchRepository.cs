using MiniStore.Domain.Entities;
namespace MiniStore.Domain.Interfaces;
public interface IPurchaseMatchRepository
{
    Task<List<PurchaseMatchRun>> GetAllAsync();
    Task<PurchaseMatchRun?> GetByVendorBillIdAsync(int vendorBillId);
    Task AddAsync(PurchaseMatchRun run);
}
