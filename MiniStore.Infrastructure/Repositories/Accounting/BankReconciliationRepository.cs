using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces.Accounting;
using MiniStore.Infrastructure.Persistence;
namespace MiniStore.Infrastructure.Repositories.Accounting;
public sealed class BankReconciliationRepository(AppDbContext context):IBankReconciliationRepository
{
 public Task<BankStatementLine?> GetStatementLineAsync(int id)=>context.BankStatementLines.FirstOrDefaultAsync(x=>x.Id==id);
 public Task<BankReconciliationAllocation?> GetAllocationAsync(int id)=>context.BankReconciliationAllocations.FirstOrDefaultAsync(x=>x.Id==id);
 public Task<List<BankReconciliationAllocation>> GetAllocationsAsync(int statementLineId)=>context.BankReconciliationAllocations.AsNoTracking().Where(x=>x.BankStatementLineId==statementLineId).OrderByDescending(x=>x.Id).ToListAsync();
 public Task<int?> GetStatementAccountIdAsync(int id)=>context.BankStatementLines.Where(x=>x.Id==id).Join(context.BankStatementImports,x=>x.BankStatementImportId,x=>x.Id,(x,i)=>i.BankAccountId).Join(context.BankAccounts,x=>x,x=>x.Id,(bankId,bank)=>(int?)bank.AccountId).FirstOrDefaultAsync();
 public async Task<(JournalEntryLine Line,JournalEntryStatus Status)?> GetJournalLineAsync(int id){var x=await context.JournalEntryLines.Where(x=>x.Id==id).Join(context.JournalEntries,x=>x.JournalEntryId,x=>x.Id,(line,entry)=>new{line,entry.Status}).FirstOrDefaultAsync();return x is null?null:(x.line,x.Status);}
 public Task<decimal> GetActiveStatementAllocationAsync(int id)=>context.BankReconciliationAllocations.Where(x=>x.BankStatementLineId==id&&x.IsActive).SumAsync(x=>(decimal?)x.Amount).ContinueWith(x=>x.Result??0m);
 public Task<decimal> GetActiveJournalAllocationAsync(int id)=>context.BankReconciliationAllocations.Where(x=>x.JournalEntryLineId==id&&x.IsActive).SumAsync(x=>(decimal?)x.Amount).ContinueWith(x=>x.Result??0m);
 public Task<List<BankJournalCandidate>> GetCandidatesAsync(int accountId,DateTime fromDate,DateTime toDate,bool debitDirection){var end=toDate.Date.AddDays(1);return context.JournalEntries.AsNoTracking().Where(x=>x.Status==JournalEntryStatus.Posted&&x.Date>=fromDate.Date&&x.Date<end).SelectMany(x=>x.Lines,(entry,line)=>new{entry,line}).Where(x=>x.line.AccountId==accountId&&(debitDirection?x.line.Debit>0:x.line.Credit>0)).OrderByDescending(x=>x.entry.Date).Select(x=>new BankJournalCandidate(x.entry.Id,x.line.Id,x.entry.EntryNumber,x.entry.Date,x.entry.Description,x.line.Debit,x.line.Credit)).Take(25).ToListAsync();}
 public async Task<BankUnreconciledSummary> GetUnreconciledSummaryAsync(DateTime toDate){var end=toDate.Date.AddDays(1);var rows=await context.BankStatementLines.AsNoTracking().Where(x=>x.TransactionDate<end).Select(x=>new{Amount=x.Debit+x.Credit,Allocated=context.BankReconciliationAllocations.Where(a=>a.BankStatementLineId==x.Id&&a.IsActive).Sum(a=>(decimal?)a.Amount)??0m}).Where(x=>x.Amount>x.Allocated).ToListAsync();return new(rows.Count,rows.Sum(x=>x.Amount-x.Allocated));}
 public Task AddAsync(BankReconciliationAllocation x)=>context.BankReconciliationAllocations.AddAsync(x).AsTask(); public Task SaveChangesAsync()=>context.SaveChangesAsync();
}
