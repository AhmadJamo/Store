using MiniStore.Domain.Entities;

namespace MiniStore.Domain.Interfaces;

public sealed record JournalEntrySearchCriteria(string? Search, DateTime? FromDate, DateTime? ToDate,
    JournalEntryStatus? Status, int Page, int PageSize);

public interface IJournalEntryRepository
{
    Task<bool> ExistsForSourceAsync(string sourceType, string sourceReference);
    Task<JournalEntry?> GetBySourceAsync(string sourceType, string sourceReference);
    Task<JournalEntry?> GetByIdAsync(int id);
    Task<(List<JournalEntry> Items, int TotalCount)> GetPagedAsync(JournalEntrySearchCriteria criteria);
    Task AddAsync(JournalEntry entry);
}
