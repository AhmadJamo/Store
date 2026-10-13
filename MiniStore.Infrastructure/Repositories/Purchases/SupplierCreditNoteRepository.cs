using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;

namespace MiniStore.Infrastructure.Repositories;

public sealed class SupplierCreditNoteRepository(AppDbContext context) : ISupplierCreditNoteRepository
{
    private IQueryable<SupplierCreditNote> Query() => context.SupplierCreditNotes.Include(x => x.Lines);
    public Task<List<SupplierCreditNote>> GetAllAsync() => Query().OrderByDescending(x => x.CreatedAtUtc).ToListAsync();
    public Task<List<SupplierCreditNote>> GetByVendorBillIdAsync(int vendorBillId) => Query().Where(x => x.VendorBillId == vendorBillId).ToListAsync();
    public Task<SupplierCreditNote?> GetByIdAsync(int id) => Query().SingleOrDefaultAsync(x => x.Id == id);
    public Task AddAsync(SupplierCreditNote value) => context.SupplierCreditNotes.AddAsync(value).AsTask();
    public Task<Dictionary<int, decimal>> GetCreditedAmountsAsync(IEnumerable<int> vendorBillIds)
    {
        var ids = vendorBillIds.Distinct().ToArray();
        return context.SupplierCreditNoteLines
            .Join(context.SupplierCreditNotes.Where(x => x.Status == SupplierCreditNoteStatus.Posted && ids.Contains(x.VendorBillId)),
                line => line.SupplierCreditNoteId, note => note.Id, (line, note) => new { note.VendorBillId, line.GrossAmount })
            .GroupBy(x => x.VendorBillId).ToDictionaryAsync(x => x.Key, x => x.Sum(y => y.GrossAmount));
    }
}
