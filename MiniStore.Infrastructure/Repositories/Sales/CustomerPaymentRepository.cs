using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;

namespace MiniStore.Infrastructure.Repositories;

public sealed class CustomerPaymentRepository(AppDbContext context) : ICustomerPaymentRepository
{
    public Task<List<CustomerPayment>> GetAllAsync(string? search = null)
    {
        var query = context.CustomerPayments.Include(x => x.Lines).AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(x => x.PaymentNumber.Contains(term) || (x.ExternalReference != null && x.ExternalReference.Contains(term)));
        }
        return query.OrderByDescending(x => x.PaymentDate).ThenByDescending(x => x.Id).ToListAsync();
    }

    public Task<CustomerPayment?> GetByIdAsync(int id) => context.CustomerPayments.Include(x => x.Lines).SingleOrDefaultAsync(x => x.Id == id);

    public Task<Dictionary<int, decimal>> GetPaidAmountsAsync(IEnumerable<int> customerInvoiceIds) => Aggregate(customerInvoiceIds, false);
    public Task<Dictionary<int, decimal>> GetReceivableCarryingAmountsAsync(IEnumerable<int> customerInvoiceIds) => Aggregate(customerInvoiceIds, true);

    private Task<Dictionary<int, decimal>> Aggregate(IEnumerable<int> invoiceIds, bool companyAmount)
    {
        var ids = invoiceIds.Distinct().ToArray();
        var lines = context.CustomerPaymentLines.Join(context.CustomerPayments.Where(x => x.Status == CustomerPaymentStatus.Posted),
            line => line.CustomerPaymentId, payment => payment.Id, (line, payment) => line).Where(x => ids.Contains(x.CustomerInvoiceId));
        return lines.GroupBy(x => x.CustomerInvoiceId).Select(x => new
            { CustomerInvoiceId = x.Key, Amount = companyAmount ? x.Sum(y => y.ReceivableCarryingAmount) : x.Sum(y => y.Amount) })
            .ToDictionaryAsync(x => x.CustomerInvoiceId, x => x.Amount);
    }

    public Task AddAsync(CustomerPayment payment) => context.CustomerPayments.AddAsync(payment).AsTask();
}
