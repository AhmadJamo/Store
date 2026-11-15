using MiniStore.Domain.Entities;
namespace MiniStore.Domain.Interfaces.Accounting;
public interface IBankReconciliationRepository
{ Task<BankStatementLine?> GetStatementLineAsync(int id); Task<BankReconciliationAllocation?> GetAllocationAsync(int id); Task<int?> GetStatementAccountIdAsync(int statementLineId); Task<(JournalEntryLine Line,JournalEntryStatus Status)?> GetJournalLineAsync(int id); Task<decimal> GetActiveStatementAllocationAsync(int statementLineId); Task<decimal> GetActiveJournalAllocationAsync(int journalLineId); Task AddAsync(BankReconciliationAllocation allocation); Task SaveChangesAsync(); }
