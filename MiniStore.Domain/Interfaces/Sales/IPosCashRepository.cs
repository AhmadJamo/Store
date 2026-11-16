using MiniStore.Domain.Entities;
namespace MiniStore.Domain.Interfaces;
public interface IPosCashRepository
{
    Task<PosCashPolicy?> GetPolicyAsync();
    Task<IReadOnlyList<PosCashDrawer>> GetDrawersAsync(int? terminalId = null);
    Task<PosCashDrawer?> GetDrawerAsync(int id);
    Task<PosCashSession?> GetSessionAsync(long id);
    Task<PosCashSession?> GetCurrentSessionAsync(int terminalId);
    Task<bool> HasSourceAsync(string sourceType, string sourceReference);
    Task AddPolicyAsync(PosCashPolicy policy);
    Task AddDrawerAsync(PosCashDrawer drawer);
    Task AddSessionAsync(PosCashSession session);
}
