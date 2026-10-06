using Microsoft.EntityFrameworkCore;using MiniStore.Domain.Entities;using MiniStore.Domain.Interfaces;using MiniStore.Infrastructure.Persistence;
namespace MiniStore.Infrastructure.Repositories;
public sealed class PutawayRuleRepository(AppDbContext db):IPutawayRuleRepository
{
 public Task<List<PutawayRule>> GetWarehouseRulesAsync(int warehouseId)=>db.PutawayRules.Where(x=>x.WarehouseId==warehouseId).OrderBy(x=>x.Priority).ToListAsync();
 public Task AddAsync(PutawayRule rule)=>db.PutawayRules.AddAsync(rule).AsTask();
 public Task SaveChangesAsync()=>db.SaveChangesAsync();
}
