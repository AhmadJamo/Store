using MiniStore.Domain.Entities;
namespace MiniStore.Domain.Interfaces;
public interface IReplenishmentRuleRepository
{
 Task<List<ReplenishmentRule>> GetAllAsync();Task<ReplenishmentRule?> GetByIdAsync(int id);
 Task<ReplenishmentRule?> GetAsync(int productId,int warehouseId);Task AddAsync(ReplenishmentRule rule);Task SaveChangesAsync();
}
