using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;

namespace MiniStore.Infrastructure.Repositories;

public sealed class SalesOrderRepository(AppDbContext context) : ISalesOrderRepository
{
    public Task<List<SalesOrder>> GetAllAsync(SalesOrderStatus? status,string? search)
    {
        var query=context.SalesOrders.AsNoTracking().AsQueryable();
        if(status.HasValue)query=query.Where(x=>x.Status==status.Value);
        if(!string.IsNullOrWhiteSpace(search)){var term=search.Trim();query=query.Where(x=>x.OrderNumber.Contains(term)||x.SourceQuotationNumber.Contains(term)||x.CustomerCode.Contains(term)||x.CustomerName.Contains(term));}
        return query.OrderByDescending(x=>x.CreatedAtUtc).ToListAsync();
    }
    public Task<SalesOrder?> GetByIdAsync(int id)=>context.SalesOrders.Include(x=>x.Lines).Include(x=>x.History).SingleOrDefaultAsync(x=>x.Id==id);
    public Task<SalesOrder?> GetBySourceQuotationIdAsync(int quotationId)=>context.SalesOrders.Include(x=>x.Lines).SingleOrDefaultAsync(x=>x.SourceQuotationId==quotationId);
    public Task AddAsync(SalesOrder order)=>context.SalesOrders.AddAsync(order).AsTask();
}
