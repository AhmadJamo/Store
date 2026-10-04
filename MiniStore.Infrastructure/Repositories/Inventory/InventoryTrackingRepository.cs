using Microsoft.EntityFrameworkCore;using MiniStore.Domain.Entities;using MiniStore.Domain.Interfaces;using MiniStore.Infrastructure.Persistence;
namespace MiniStore.Infrastructure.Repositories;
public sealed class InventoryTrackingRepository(AppDbContext db):IInventoryTrackingRepository
{
 public Task<List<InventoryTrackingBalance>> GetBalancesAsync()=>db.InventoryTrackingBalances.AsNoTracking().OrderBy(x=>x.ExpirationDate).ThenBy(x=>x.Identifier).ToListAsync();
 public Task<InventoryTrackingBalance?> GetByIdForUpdateAsync(long id)=>db.InventoryTrackingBalances.SingleOrDefaultAsync(x=>x.Id==id);
 public Task<List<InventoryTrackingTransaction>> GetTransactionsAsync()=>db.InventoryTrackingTransactions.AsNoTracking().OrderByDescending(x=>x.CreatedAt).ToListAsync();
 public Task<bool> IdentifierExistsAsync(int productId,ProductTrackingPolicy policy,string identifier)=>db.InventoryTrackingBalances.AnyAsync(x=>x.ProductId==productId&&x.Policy==policy&&x.Identifier==identifier.Trim().ToUpper());
 public Task<InventoryTrackingBalance?> GetBalanceAsync(int productId,int warehouseId,int? locationId,string identifier)=>db.InventoryTrackingBalances.SingleOrDefaultAsync(x=>x.ProductId==productId&&x.WarehouseId==warehouseId&&x.StorageLocationId==locationId&&x.Identifier==identifier.Trim().ToUpper());
 public Task<List<InventoryTrackingBalance>> GetAvailableForUpdateAsync(int productId,int warehouseId,int? locationId=null)=>db.InventoryTrackingBalances.Where(x=>x.ProductId==productId&&x.WarehouseId==warehouseId&&(!locationId.HasValue||x.StorageLocationId==locationId)&&x.Quantity>0&&x.Status==InventoryTrackingStatus.Available).OrderBy(x=>x.ExpirationDate==null).ThenBy(x=>x.ExpirationDate).ThenBy(x=>x.ReceivedAt).ToListAsync();
 public Task<List<InventoryTrackingBalance>> GetByIdentifierForUpdateAsync(int productId,string identifier)=>db.InventoryTrackingBalances.Where(x=>x.ProductId==productId&&x.Identifier==identifier.Trim().ToUpper()).OrderBy(x=>x.ReceivedAt).ToListAsync();
 public Task AddBalanceAsync(InventoryTrackingBalance balance)=>db.InventoryTrackingBalances.AddAsync(balance).AsTask();
 public Task AddTransactionAsync(InventoryTrackingTransaction transaction)=>db.InventoryTrackingTransactions.AddAsync(transaction).AsTask();
}
