using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;

namespace MiniStore.Infrastructure.Repositories;

public sealed class RecurringJournalTemplateRepository(AppDbContext context) : IRecurringJournalTemplateRepository
{
    public Task<List<RecurringJournalTemplate>> GetAllAsync() => context.RecurringJournalTemplates.AsNoTracking()
        .Include(x => x.Lines).OrderBy(x => x.Name).ToListAsync();
    public Task<RecurringJournalTemplate?> GetByIdAsync(int id) => context.RecurringJournalTemplates
        .Include(x => x.Lines).SingleOrDefaultAsync(x => x.Id == id);
    public Task AddAsync(RecurringJournalTemplate template) => context.RecurringJournalTemplates.AddAsync(template).AsTask();
    public void SetOriginalRowVersion(RecurringJournalTemplate template, byte[] rowVersion) =>
        context.Entry(template).Property(x => x.RowVersion).OriginalValue = rowVersion;
}
