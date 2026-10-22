using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;

namespace MiniStore.Infrastructure.Repositories;

public sealed class SalesQuotationTemplateRepository(AppDbContext context) : ISalesQuotationTemplateRepository
{
    private IQueryable<SalesQuotationTemplate> Query(bool tracking = false)
    {
        var query = context.SalesQuotationTemplates.Include(x => x.Items).AsQueryable();
        return tracking ? query : query.AsNoTracking();
    }

    public Task<List<SalesQuotationTemplate>> GetAllAsync(SalesQuotationTemplateStatus? status, string? search)
    {
        var query = Query();
        if (status.HasValue) query = query.Where(x => x.Status == status.Value);
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(x => x.Code.Contains(term) || x.Name.Contains(term) ||
                (x.Category != null && x.Category.Contains(term)) || (x.Tags != null && x.Tags.Contains(term)));
        }
        return query.OrderBy(x => x.Code).ThenByDescending(x => x.Version).ToListAsync();
    }

    public Task<SalesQuotationTemplate?> GetByIdAsync(int id, bool tracking = true) => Query(tracking).SingleOrDefaultAsync(x => x.Id == id);
    public Task<SalesQuotationTemplate?> GetDefaultAsync() => Query().SingleOrDefaultAsync(x => x.Status == SalesQuotationTemplateStatus.Active && x.IsDefault);
    public Task<SalesQuotationTemplate?> GetByCodeVersionAsync(string code, int version) => Query().SingleOrDefaultAsync(x => x.Code == code.Trim().ToUpper() && x.Version == version);
    public Task<List<SalesQuotationTemplate>> GetActiveByCodeAsync(string code) => Query(true).Where(x => x.Code == code.Trim().ToUpper() && x.Status == SalesQuotationTemplateStatus.Active).ToListAsync();
    public Task<List<SalesQuotationTemplate>> GetActiveDefaultsAsync() => Query(true).Where(x => x.Status == SalesQuotationTemplateStatus.Active && x.IsDefault).ToListAsync();
    public Task AddAsync(SalesQuotationTemplate template) => context.SalesQuotationTemplates.AddAsync(template).AsTask();
}
