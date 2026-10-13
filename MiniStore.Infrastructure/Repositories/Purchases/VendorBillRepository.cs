using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;
namespace MiniStore.Infrastructure.Repositories;
public sealed class VendorBillRepository(AppDbContext context) : IVendorBillRepository
{
    public Task<List<VendorBill>> GetAllAsync(string? search)
    {
        var query = context.VendorBills.Include(x => x.Lines).Include(x => x.Attachments).AsQueryable();
        if (!string.IsNullOrWhiteSpace(search)) query = query.Where(x => x.BillNumber.Contains(search) || x.SupplierInvoiceNumber.Contains(search));
        return query.OrderByDescending(x => x.CreatedAtUtc).ToListAsync();
    }
    public Task<List<VendorBill>> GetByPurchaseOrderIdAsync(int purchaseOrderId) => context.VendorBills.Include(x => x.Lines).Include(x => x.Attachments).Where(x => x.PurchaseOrderId == purchaseOrderId).ToListAsync();
    public Task<List<VendorBill>> GetBySupplierIdAsync(int supplierId) => context.VendorBills.Include(x => x.Lines).Include(x => x.Attachments).Where(x => x.SupplierId == supplierId).ToListAsync();
    public Task<VendorBill?> GetByIdAsync(int id) => context.VendorBills.Include(x => x.Lines).Include(x => x.Attachments).FirstOrDefaultAsync(x => x.Id == id);
    public Task<bool> ExistsActiveSupplierInvoiceAsync(int supplierId, string normalizedSupplierInvoiceNumber, int? excludingBillId = null) => context.VendorBills.AnyAsync(x => x.SupplierId == supplierId && x.NormalizedSupplierInvoiceNumber == normalizedSupplierInvoiceNumber && x.Status != VendorBillStatus.Cancelled && (!excludingBillId.HasValue || x.Id != excludingBillId.Value));
    public Task<VendorBillAttachment?> GetAttachmentAsync(int billId, int attachmentId) => context.Set<VendorBillAttachment>().FirstOrDefaultAsync(x => x.VendorBillId == billId && x.Id == attachmentId);
    public Task AddAsync(VendorBill bill) => context.VendorBills.AddAsync(bill).AsTask();
}
