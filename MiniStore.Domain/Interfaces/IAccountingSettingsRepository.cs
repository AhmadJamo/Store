using MiniStore.Domain.Entities;

namespace MiniStore.Domain.Interfaces;

public interface IAccountingSettingsRepository
{
    Task<AccountingSettings?> GetAsync();
    Task AddAsync(AccountingSettings settings);
    Task SaveChangesAsync();
}
