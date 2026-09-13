using MiniStore.Domain.Entities;

namespace MiniStore.Domain.Interfaces;

public interface IJournalEntryRepository
{
    Task<bool> ExistsForSourceAsync(string sourceType, string sourceReference);
    Task AddAsync(JournalEntry entry);
}
