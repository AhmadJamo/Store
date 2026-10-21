using MiniStore.Domain.Entities;

namespace MiniStore.Domain.Interfaces;

public interface IJournalEntryRepository
{
    Task<bool> ExistsForSourceAsync(string sourceType, string sourceReference);
    Task<JournalEntry?> GetBySourceAsync(string sourceType, string sourceReference);
    Task<JournalEntry?> GetByIdAsync(int id);
    Task AddAsync(JournalEntry entry);
}
