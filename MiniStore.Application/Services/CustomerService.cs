using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
namespace MiniStore.Application.Services;
public class CustomerService(ICustomerRepository customers, IAccountRepository accounts)
{
    public Task<List<Customer>> GetAllAsync()=>customers.GetAllAsync();
    public async Task CreateAsync(string name,int accountId) { var account = await accounts.GetByIdAsync(accountId); if(account is null) throw new InvalidOperationException("Selected customer account was not found."); if(!account.ParentAccountId.HasValue) throw new InvalidOperationException("Customer receivable account must be a subaccount in the chart of accounts."); if((await customers.GetAllAsync()).Any(x=>x.Name.Equals(name.Trim(),StringComparison.OrdinalIgnoreCase))) throw new InvalidOperationException("Customer name already exists."); await customers.AddAsync(new Customer(name,accountId)); await customers.SaveChangesAsync(); }
}
