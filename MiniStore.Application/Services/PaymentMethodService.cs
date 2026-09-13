using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
namespace MiniStore.Application.Services;
public class PaymentMethodService(IPaymentMethodRepository methods, IAccountRepository accounts)
{
    public Task<List<PaymentMethod>> GetAllAsync() => methods.GetAllAsync();
    public async Task CreateAsync(string name, int accountId)
    {
        if (await accounts.GetByIdAsync(accountId) is null) throw new InvalidOperationException("Selected settlement account was not found.");
        if ((await methods.GetAllAsync()).Any(x => x.Name.Equals(name.Trim(), StringComparison.OrdinalIgnoreCase))) throw new InvalidOperationException("Payment method name already exists.");
        await methods.AddAsync(new PaymentMethod(name, accountId)); await methods.SaveChangesAsync();
    }
}
