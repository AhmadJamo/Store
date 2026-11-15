using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces.Accounting;
using MiniStore.Infrastructure.Persistence;
namespace MiniStore.Infrastructure.Repositories.Accounting;
public sealed class BankReconciliationRepository(AppDbContext context):IBankReconciliationRepository
{
 public Task<BankStatementLine?> GetStatementLineAsync(int id)=>context.BankStatementLines.FirstOrDefaultAsync(x=>x.Id==id);
 public Task<int?> GetStatementAccountIdAsync(int id)=>context.BankStatementLines.Where(x=>x.Id==id).Join(context.BankStatementImports,x=>x.BankStatementImportId,x=>x.Id,(x,i)=>i.BankAccountId).Join(context.BankAccounts,x=>x,x=>x.Id,(bankId,bank)=>(int?)bank.AccountId).FirstOrDefaultAsync();
 public async Task<(JournalEntryLine Line,JournalEntryStatus Status)?> GetJournalLineAsync(int id){var x=await context.JournalEntryLines.Where(x=>x.Id==id).Join(context.JournalEntries,x=>x.JournalEntryId,x=>x.Id,(line,entry)=>new{line,entry.Status}).FirstOrDefaultAsync();return x is null?null:(x.line,x.Status);}
 public Task<decimal> GetActiveStatementAllocationAsync(int id)=>context.BankReconciliationAllocations.Where(x=>x.BankStatementLineId==id&&x.IsActive).SumAsync(x=>(decimal?)x.Amount).ContinueWith(x=>x.Result??0m);
 public Task<decimal> GetActiveJournalAllocationAsync(int id)=>context.BankReconciliationAllocations.Where(x=>x.JournalEntryLineId==id&&x.IsActive).SumAsync(x=>(decimal?)x.Amount).ContinueWith(x=>x.Result??0m);
 public Task AddAsync(BankReconciliationAllocation x)=>context.BankReconciliationAllocations.AddAsync(x).AsTask(); public Task SaveChangesAsync()=>context.SaveChangesAsync();
}
