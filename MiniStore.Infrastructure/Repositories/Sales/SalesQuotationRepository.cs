using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;

namespace MiniStore.Infrastructure.Repositories;

public sealed class SalesQuotationRepository(AppDbContext context) : ISalesQuotationRepository
{
    public Task<List<SalesQuotation>> GetAllAsync(SalesQuotationStatus? status, string? search)
    {
        var query = context.SalesQuotations.Include(x => x.Lines).AsNoTracking().AsQueryable();
        if (status.HasValue) query = query.Where(x => x.Status == status.Value);
        if (!string.IsNullOrWhiteSpace(search)) { var term = search.Trim(); query = query.Where(x => x.QuotationNumber.Contains(term) || x.CustomerCode.Contains(term) || x.CustomerName.Contains(term)); }
        return query.OrderByDescending(x => x.CreatedAtUtc).ToListAsync();
    }
    public Task<SalesQuotation?> GetByIdAsync(int id) => context.SalesQuotations.Include(x => x.Lines).Include(x => x.History).SingleOrDefaultAsync(x => x.Id == id);
    public Task<SalesQuotation?> GetByRequestKeyAsync(string requestKey) => context.SalesQuotations.Include(x => x.Lines).SingleOrDefaultAsync(x => x.RequestKey == requestKey.ToLower());
    public Task AddAsync(SalesQuotation quotation) => context.SalesQuotations.AddAsync(quotation).AsTask();
}
