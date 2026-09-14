using MiniStore.Domain.Entities;

namespace MiniStore.Domain.Interfaces;

public interface IInventorySettingsRepository
{
    Task<InventorySettings?> GetAsync();
    Task AddAsync(InventorySettings settings);
    Task SaveChangesAsync();
}
