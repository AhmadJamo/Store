using MiniStore.Domain.Entities;
namespace MiniStore.Domain.Interfaces.Accounting;
public interface IBankStatementRepository { Task<List<BankAccount>> GetAccountsAsync(); Task<BankAccount?> GetAccountAsync(int id); Task<bool> ImportExistsAsync(int bankAccountId,string hash); Task<bool> LineExistsAsync(int bankAccountId,string fingerprint); Task AddAccountAsync(BankAccount account); Task AddImportAsync(BankStatementImport import); Task<List<BankStatementImport>> GetImportsAsync(); Task SaveChangesAsync(); }
