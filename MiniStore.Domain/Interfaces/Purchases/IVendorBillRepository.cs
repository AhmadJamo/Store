using MiniStore.Domain.Entities;
namespace MiniStore.Domain.Interfaces;
public interface IVendorBillRepository
{
    Task<List<VendorBill>> GetAllAsync(string? search);
    Task<List<VendorBill>> GetByPurchaseOrderIdAsync(int purchaseOrderId);
    Task<List<VendorBill>> GetBySupplierIdAsync(int supplierId);
    Task<VendorBill?> GetByIdAsync(int id);
    Task<bool> ExistsActiveSupplierInvoiceAsync(int supplierId, string normalizedSupplierInvoiceNumber, int? excludingBillId = null);
    Task<VendorBillAttachment?> GetAttachmentAsync(int billId, int attachmentId);
    Task AddAsync(VendorBill bill);
}
