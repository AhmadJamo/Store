using Microsoft.EntityFrameworkCore;using MiniStore.Domain.Entities;using MiniStore.Domain.Interfaces;using MiniStore.Infrastructure.Persistence;
namespace MiniStore.Infrastructure.Repositories;
public sealed class ReplenishmentRuleRepository(AppDbContext db):IReplenishmentRuleRepository
{
 public Task<List<ReplenishmentRule>> GetAllAsync()=>db.ReplenishmentRules.OrderBy(x=>x.WarehouseId).ThenBy(x=>x.ProductId).ToListAsync();
 public Task<ReplenishmentRule?> GetByIdAsync(int id)=>db.ReplenishmentRules.FirstOrDefaultAsync(x=>x.Id==id);
 public Task<ReplenishmentRule?> GetAsync(int productId,int warehouseId)=>db.ReplenishmentRules.FirstOrDefaultAsync(x=>x.ProductId==productId&&x.WarehouseId==warehouseId);
 public Task AddAsync(ReplenishmentRule rule)=>db.ReplenishmentRules.AddAsync(rule).AsTask();public Task SaveChangesAsync()=>db.SaveChangesAsync();
}
