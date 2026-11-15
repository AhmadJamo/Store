using MiniStore.Domain.Entities;
namespace MiniStore.Domain.Interfaces.Accounting;
public sealed record BankJournalCandidate(int JournalEntryId,int JournalEntryLineId,string EntryNumber,DateTime Date,string? Description,decimal Debit,decimal Credit);
public interface IBankReconciliationRepository
{
 Task<BankStatementLine?> GetStatementLineAsync(int id);
 Task<BankReconciliationAllocation?> GetAllocationAsync(int id);
 Task<List<BankReconciliationAllocation>> GetAllocationsAsync(int statementLineId);
 Task<int?> GetStatementAccountIdAsync(int statementLineId);
 Task<(JournalEntryLine Line,JournalEntryStatus Status)?> GetJournalLineAsync(int id);
 Task<decimal> GetActiveStatementAllocationAsync(int statementLineId);
 Task<decimal> GetActiveJournalAllocationAsync(int journalLineId);
 Task<List<BankJournalCandidate>> GetCandidatesAsync(int accountId,DateTime fromDate,DateTime toDate,bool debitDirection);
 Task AddAsync(BankReconciliationAllocation allocation);
 Task SaveChangesAsync();
}
