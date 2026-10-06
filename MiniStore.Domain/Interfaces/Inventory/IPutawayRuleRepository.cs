using MiniStore.Domain.Entities;
namespace MiniStore.Domain.Interfaces;
public interface IPutawayRuleRepository
{
    Task<List<PutawayRule>> GetWarehouseRulesAsync(int warehouseId);
    Task AddAsync(PutawayRule rule);
    Task SaveChangesAsync();
}
