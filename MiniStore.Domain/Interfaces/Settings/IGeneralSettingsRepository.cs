using MiniStore.Domain.Entities;

namespace MiniStore.Domain.Interfaces;

public interface IGeneralSettingsRepository
{
    Task<GeneralSettings?> GetAsync();

    Task AddAsync(GeneralSettings settings);

    Task SaveChangesAsync();
}