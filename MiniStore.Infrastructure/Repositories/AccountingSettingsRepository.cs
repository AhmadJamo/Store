using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;

namespace MiniStore.Infrastructure.Repositories;

public class AccountingSettingsRepository(AppDbContext context) : IAccountingSettingsRepository
{
    public Task<AccountingSettings?> GetAsync() =>
        context.AccountingSettings.SingleOrDefaultAsync(x => x.SingletonKey == 1);

    public Task AddAsync(AccountingSettings settings) => context.AccountingSettings.AddAsync(settings).AsTask();

    public Task SaveChangesAsync() => context.SaveChangesAsync();
}
