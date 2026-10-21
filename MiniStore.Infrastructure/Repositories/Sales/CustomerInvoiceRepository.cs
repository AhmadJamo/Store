using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;

namespace MiniStore.Infrastructure.Repositories;

public sealed class CustomerInvoiceRepository(AppDbContext context) : ICustomerInvoiceRepository
{
    private IQueryable<CustomerInvoice> Aggregate(bool tracking = false)
    {
        var query = context.CustomerInvoices.Include(x => x.Lines).ThenInclude(x => x.SourceAllocations).Include(x => x.History);
        return tracking ? query : query.AsNoTracking();
    }

    public Task<List<CustomerInvoice>> GetAllAsync(CustomerInvoiceStatus? status = null, string? search = null)
    {
        var query = Aggregate();
        if (status.HasValue) query = query.Where(x => x.Status == status.Value);
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(x => x.InvoiceNumber.Contains(term) || x.CustomerCode.Contains(term) || x.CustomerName.Contains(term));
        }
        return query.OrderByDescending(x => x.InvoiceDate).ThenByDescending(x => x.Id).ToListAsync();
    }

    public Task<List<CustomerInvoice>> GetBySalesOrderAsync(int salesOrderId) => Aggregate()
        .Where(x => x.SalesOrderId == salesOrderId).OrderByDescending(x => x.Id).ToListAsync();
    public Task<List<CustomerInvoice>> GetByCustomerAsync(int customerId) => Aggregate()
        .Where(x => x.CustomerId == customerId).OrderBy(x => x.DueDate).ThenBy(x => x.Id).ToListAsync();
    public Task<CustomerInvoice?> GetByIdAsync(int id) => Aggregate(true).SingleOrDefaultAsync(x => x.Id == id);
    public Task<CustomerInvoice?> GetByRequestKeyAsync(string requestKey) => Aggregate()
        .SingleOrDefaultAsync(x => x.RequestKey == requestKey.Trim().ToLower());
    public Task AddAsync(CustomerInvoice invoice) => context.CustomerInvoices.AddAsync(invoice).AsTask();
}
