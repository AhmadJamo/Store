using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;

namespace MiniStore.Infrastructure.Repositories;

public class JournalEntryRepository(AppDbContext context) : IJournalEntryRepository
{
    public Task<bool> ExistsForSourceAsync(string sourceType, string sourceReference) =>
        context.JournalEntries.AnyAsync(x => x.SourceType == sourceType && x.SourceReference == sourceReference);

    public Task<JournalEntry?> GetBySourceAsync(string sourceType, string sourceReference) =>
        context.JournalEntries.Include(x => x.Lines)
            .SingleOrDefaultAsync(x => x.SourceType == sourceType && x.SourceReference == sourceReference);

    public Task<JournalEntry?> GetByIdAsync(int id) => context.JournalEntries.AsNoTracking()
        .Include(x => x.Lines).SingleOrDefaultAsync(x => x.Id == id);

    public async Task<(List<JournalEntry> Items, int TotalCount)> GetPagedAsync(JournalEntrySearchCriteria criteria)
    {
        var query = context.JournalEntries.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(criteria.Search))
        {
            var term = criteria.Search.Trim();
            query = query.Where(x => x.EntryNumber.Contains(term)
                || (x.Description != null && x.Description.Contains(term))
                || (x.SourceType != null && x.SourceType.Contains(term))
                || (x.SourceReference != null && x.SourceReference.Contains(term)));
        }
        if (criteria.FromDate.HasValue) query = query.Where(x => x.Date >= criteria.FromDate.Value.Date);
        if (criteria.ToDate.HasValue)
        {
            var exclusiveEnd = criteria.ToDate.Value.Date.AddDays(1);
            query = query.Where(x => x.Date < exclusiveEnd);
        }
        if (criteria.Status.HasValue) query = query.Where(x => x.Status == criteria.Status.Value);
        var totalCount = await query.CountAsync();
        var items = await query.OrderByDescending(x => x.Date).ThenByDescending(x => x.Id)
            .Skip((criteria.Page - 1) * criteria.PageSize).Take(criteria.PageSize)
            .Include(x => x.Lines).ToListAsync();
        return (items, totalCount);
    }

    public Task AddAsync(JournalEntry entry) => context.JournalEntries.AddAsync(entry).AsTask();
}
