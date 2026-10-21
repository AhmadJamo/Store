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

    public Task AddAsync(JournalEntry entry) => context.JournalEntries.AddAsync(entry).AsTask();
}
