using MiniStore.Domain.Entities;

namespace MiniStore.Domain.Interfaces;

public interface ISupplierCreditNoteRepository
{
    Task<List<SupplierCreditNote>> GetAllAsync();
    Task<List<SupplierCreditNote>> GetByVendorBillIdAsync(int vendorBillId);
    Task<SupplierCreditNote?> GetByIdAsync(int id);
    Task<Dictionary<int, decimal>> GetCreditedAmountsAsync(IEnumerable<int> vendorBillIds);
    Task AddAsync(SupplierCreditNote value);
}
