using MiniStore.Domain.Entities;

namespace MiniStore.Domain.Interfaces;

public interface IDiscountSettingsRepository
{
    Task<DiscountSettings?> GetAsync();
    Task AddAsync(DiscountSettings settings);
    Task SaveChangesAsync();
}