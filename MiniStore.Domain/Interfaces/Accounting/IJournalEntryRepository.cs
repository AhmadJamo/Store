using MiniStore.Domain.Entities;

namespace MiniStore.Domain.Interfaces;

public sealed record JournalEntrySearchCriteria(string? Search, DateTime? FromDate, DateTime? ToDate,
    JournalEntryStatus? Status, int? AccountId, string? SourceTypePrefix, int Page, int PageSize);

public sealed record FinancialAccountMovement(int AccountId, decimal OpeningNet, decimal PeriodDebit,
    decimal PeriodCredit, decimal ClosingNet);

public interface IJournalEntryRepository
{
    Task<bool> ExistsForSourceAsync(string sourceType, string sourceReference);
    Task<JournalEntry?> GetBySourceAsync(string sourceType, string sourceReference);
    Task<JournalEntry?> GetByIdAsync(int id);
    Task<JournalEntry?> GetByIdForUpdateAsync(int id);
    Task<(List<JournalEntry> Items, int TotalCount)> GetPagedAsync(JournalEntrySearchCriteria criteria);
    Task<List<FinancialAccountMovement>> GetFinancialMovementsAsync(DateTime fromDate, DateTime toDate);
    Task AddAsync(JournalEntry entry);
}
