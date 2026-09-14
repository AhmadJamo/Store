using MiniStore.Domain.Entities;

namespace MiniStore.Domain.Interfaces;

public interface IPosTerminalSettingsRepository
{
    Task<List<PosTerminalSettings>> GetAllAsync();
    Task<PosTerminalSettings?> GetAsync(int posTerminalId);
    Task AddAsync(PosTerminalSettings settings);
    Task SaveChangesAsync();
}
