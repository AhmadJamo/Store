using MiniStore.Domain.Entities;
namespace MiniStore.Domain.Interfaces.Accounting;
public interface IBankReconciliationRepository
{ Task<BankStatementLine?> GetStatementLineAsync(int id); Task<JournalEntryLine?> GetJournalLineAsync(int id); Task<decimal> GetActiveStatementAllocationAsync(int statementLineId); Task<decimal> GetActiveJournalAllocationAsync(int journalLineId); Task AddAsync(BankReconciliationAllocation allocation); Task SaveChangesAsync(); }
