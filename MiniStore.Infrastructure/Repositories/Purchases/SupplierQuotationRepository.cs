using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;

namespace MiniStore.Infrastructure.Repositories;

public sealed class SupplierQuotationRepository(AppDbContext context) : ISupplierQuotationRepository
{
    public Task<List<SupplierQuotation>> GetBySourcingEventAsync(int sourcingEventId) => context.SupplierQuotations
        .Include(x => x.Lines).Where(x => x.PurchaseSourcingEventId == sourcingEventId)
        .OrderBy(x => x.SupplierId).ToListAsync();
    public Task<SupplierQuotation?> GetByIdAsync(int id) => context.SupplierQuotations.Include(x => x.Lines)
        .FirstOrDefaultAsync(x => x.Id == id);
    public Task<SupplierQuotation?> GetBySourcingAndSupplierAsync(int sourcingEventId, int supplierId) =>
        context.SupplierQuotations.FirstOrDefaultAsync(x => x.PurchaseSourcingEventId == sourcingEventId && x.SupplierId == supplierId);
    public Task<PurchaseQuotationAward?> GetAwardAsync(int sourcingEventId) => context.PurchaseQuotationAwards
        .SingleOrDefaultAsync(x => x.PurchaseSourcingEventId == sourcingEventId);
    public Task AddAsync(SupplierQuotation quotation) => context.SupplierQuotations.AddAsync(quotation).AsTask();
    public Task AddAwardAsync(PurchaseQuotationAward award) => context.PurchaseQuotationAwards.AddAsync(award).AsTask();
}
