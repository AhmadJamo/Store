using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Application.Dtos.Settings;
namespace MiniStore.Application.Services;
public class PaymentMethodService(IPaymentMethodRepository methods, IAccountRepository accounts)
{
    public Task<List<PaymentMethod>> GetAllAsync() => methods.GetAllAsync();
    public async Task<List<PaymentMethodRowDto>> GetRowsAsync() { var accountRows=(await accounts.GetAllAsync()).ToDictionary(x=>x.Id); return (await methods.GetAllAsync()).Select(x=>new PaymentMethodRowDto{Id=x.Id,Name=x.Name,AccountName=accountRows.TryGetValue(x.AccountId,out var account)?$"{account.Code} — {account.Name}":string.Empty,IsActive=x.IsActive}).ToList(); }
    public async Task CreateAsync(string name, int accountId)
    {
        if (await accounts.GetByIdAsync(accountId) is null) throw new InvalidOperationException("Selected settlement account was not found.");
        if ((await methods.GetAllAsync()).Any(x => x.Name.Equals(name.Trim(), StringComparison.OrdinalIgnoreCase))) throw new InvalidOperationException("Payment method name already exists.");
        await methods.AddAsync(new PaymentMethod(name, accountId)); await methods.SaveChangesAsync();
    }
}
