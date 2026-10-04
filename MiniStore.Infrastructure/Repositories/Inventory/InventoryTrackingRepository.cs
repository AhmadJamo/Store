using Microsoft.EntityFrameworkCore;using MiniStore.Domain.Entities;using MiniStore.Domain.Interfaces;using MiniStore.Infrastructure.Persistence;
namespace MiniStore.Infrastructure.Repositories;
public sealed class InventoryTrackingRepository(AppDbContext db):IInventoryTrackingRepository
{
 public Task<List<InventoryTrackingBalance>> GetBalancesAsync()=>db.InventoryTrackingBalances.AsNoTracking().OrderBy(x=>x.ExpirationDate).ThenBy(x=>x.Identifier).ToListAsync();
 public Task<List<InventoryTrackingTransaction>> GetTransactionsAsync()=>db.InventoryTrackingTransactions.AsNoTracking().OrderByDescending(x=>x.CreatedAt).ToListAsync();
 public Task<bool> IdentifierExistsAsync(int productId,ProductTrackingPolicy policy,string identifier)=>db.InventoryTrackingBalances.AnyAsync(x=>x.ProductId==productId&&x.Policy==policy&&x.Identifier==identifier.Trim().ToUpper());
 public Task AddBalanceAsync(InventoryTrackingBalance balance)=>db.InventoryTrackingBalances.AddAsync(balance).AsTask();
 public Task AddTransactionAsync(InventoryTrackingTransaction transaction)=>db.InventoryTrackingTransactions.AddAsync(transaction).AsTask();
}
