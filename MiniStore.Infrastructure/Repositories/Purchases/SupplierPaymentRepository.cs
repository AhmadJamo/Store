using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;

namespace MiniStore.Infrastructure.Repositories;

public sealed class SupplierPaymentRepository(AppDbContext context) : ISupplierPaymentRepository
{
    public Task<List<SupplierPayment>> GetAllAsync(string? search)
    {
        var query = context.Set<SupplierPayment>().Include(x => x.Lines).AsQueryable();
        if (!string.IsNullOrWhiteSpace(search)) query = query.Where(x => x.PaymentNumber.Contains(search) || (x.ExternalReference != null && x.ExternalReference.Contains(search)));
        return query.OrderByDescending(x => x.PaymentDate).ThenByDescending(x => x.Id).ToListAsync();
    }

    public Task<SupplierPayment?> GetByIdAsync(int id) => context.Set<SupplierPayment>().Include(x => x.Lines).FirstOrDefaultAsync(x => x.Id == id);

    public Task<Dictionary<int, decimal>> GetPaidAmountsAsync(IEnumerable<int> vendorBillIds)
    {
        var ids = vendorBillIds.Distinct().ToArray();
        return context.Set<SupplierPaymentLine>().Where(x => ids.Contains(x.VendorBillId))
            .GroupBy(x => x.VendorBillId).Select(x => new { VendorBillId = x.Key, Amount = x.Sum(y => y.Amount) })
            .ToDictionaryAsync(x => x.VendorBillId, x => x.Amount);
    }

    public Task AddAsync(SupplierPayment payment) => context.Set<SupplierPayment>().AddAsync(payment).AsTask();
}
