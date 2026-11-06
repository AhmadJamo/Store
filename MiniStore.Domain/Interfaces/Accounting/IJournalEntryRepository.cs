using MiniStore.Domain.Entities;

namespace MiniStore.Domain.Interfaces;

public sealed record JournalEntrySearchCriteria(string? Search, DateTime? FromDate, DateTime? ToDate,
    JournalEntryStatus? Status, int? AccountId, string? SourceTypePrefix, int Page, int PageSize);

public sealed record FinancialAccountMovement(int AccountId, decimal OpeningNet, decimal PeriodDebit,
    decimal PeriodCredit, decimal ClosingNet);
public sealed record AccountBalance(int AccountId, decimal Debit, decimal Credit);
public sealed record CashFlowJournalMovement(int EntryId, string EntryNumber, DateTime Date,
    string? Description, string? SourceType, string? SourceReference, decimal Amount,
    IReadOnlyList<AccountType> CounterpartTypes);

public interface IJournalEntryRepository
{
    Task<bool> ExistsForSourceAsync(string sourceType, string sourceReference);
    Task<JournalEntry?> GetBySourceAsync(string sourceType, string sourceReference);
    Task<JournalEntry?> GetByIdAsync(int id);
    Task<JournalEntry?> GetByIdForUpdateAsync(int id);
    Task<(List<JournalEntry> Items, int TotalCount)> GetPagedAsync(JournalEntrySearchCriteria criteria);
    Task<List<FinancialAccountMovement>> GetFinancialMovementsAsync(DateTime fromDate, DateTime toDate);
    Task<List<AccountBalance>> GetAccountBalancesAsOfAsync(IEnumerable<int> accountIds, DateTime asOfDate);
    Task<List<CashFlowJournalMovement>> GetCashFlowMovementsAsync(IEnumerable<int> cashAccountIds,
        DateTime fromDate, DateTime toDate);
    Task<int> CountDraftEntriesAsync(DateTime toDate);
    Task AddAsync(JournalEntry entry);
}
