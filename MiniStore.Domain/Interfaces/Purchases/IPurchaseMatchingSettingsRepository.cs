using MiniStore.Domain.Entities;
namespace MiniStore.Domain.Interfaces;
public interface IPurchaseMatchingSettingsRepository
{
    Task<PurchaseMatchingSettings?> GetAsync();
    Task AddAsync(PurchaseMatchingSettings settings);
    Task SaveChangesAsync();
}
