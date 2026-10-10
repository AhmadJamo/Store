using MiniStore.Domain.Entities;
namespace MiniStore.Domain.Interfaces;
public interface IVendorBillRepository
{
    Task<List<VendorBill>> GetAllAsync(string? search);
    Task<List<VendorBill>> GetByPurchaseOrderIdAsync(int purchaseOrderId);
    Task<List<VendorBill>> GetBySupplierIdAsync(int supplierId);
    Task<VendorBill?> GetByIdAsync(int id);
    Task<bool> ExistsActiveSupplierInvoiceAsync(int supplierId, string normalizedSupplierInvoiceNumber);
    Task AddAsync(VendorBill bill);
}
