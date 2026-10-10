using MiniStore.Domain.Entities;

namespace MiniStore.Domain.Interfaces;

public interface ISupplierPaymentRepository
{
    Task<List<SupplierPayment>> GetAllAsync(string? search);
    Task<SupplierPayment?> GetByIdAsync(int id);
    Task<Dictionary<int, decimal>> GetPaidAmountsAsync(IEnumerable<int> vendorBillIds);
    Task AddAsync(SupplierPayment payment);
}
