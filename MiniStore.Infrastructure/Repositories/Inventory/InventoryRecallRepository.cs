using Microsoft.EntityFrameworkCore;using MiniStore.Domain.Entities;using MiniStore.Domain.Interfaces;using MiniStore.Infrastructure.Persistence;
namespace MiniStore.Infrastructure.Repositories;
public sealed class InventoryRecallRepository(AppDbContext db):IInventoryRecallRepository
{
 public Task<List<InventoryRecall>> GetAllAsync()=>db.InventoryRecalls.AsNoTracking().OrderByDescending(x=>x.CreatedAt).ToListAsync();
 public Task<InventoryRecall?> GetByIdForUpdateAsync(long id)=>db.InventoryRecalls.SingleOrDefaultAsync(x=>x.Id==id);
 public Task<bool> HasActiveAsync(int productId,string identifier)=>db.InventoryRecalls.AnyAsync(x=>x.ProductId==productId&&x.Identifier==identifier.Trim().ToUpper()&&x.Status==InventoryRecallStatus.Active);
 public Task<bool> ReferenceExistsAsync(string reference)=>db.InventoryRecalls.AnyAsync(x=>x.Reference==reference.Trim().ToUpper());
 public Task AddAsync(InventoryRecall recall)=>db.InventoryRecalls.AddAsync(recall).AsTask();
}
