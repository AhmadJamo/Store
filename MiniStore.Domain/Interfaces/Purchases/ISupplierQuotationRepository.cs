using MiniStore.Domain.Entities;

namespace MiniStore.Domain.Interfaces;

public interface ISupplierQuotationRepository
{
    Task<List<SupplierQuotation>> GetBySourcingEventAsync(int sourcingEventId);
    Task<SupplierQuotation?> GetByIdAsync(int id);
    Task<SupplierQuotation?> GetBySourcingAndSupplierAsync(int sourcingEventId, int supplierId);
    Task<PurchaseQuotationAward?> GetAwardAsync(int sourcingEventId);
    Task AddAsync(SupplierQuotation quotation);
    Task AddAwardAsync(PurchaseQuotationAward award);
}
