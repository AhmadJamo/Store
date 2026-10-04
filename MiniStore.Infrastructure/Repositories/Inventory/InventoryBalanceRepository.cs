using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;

namespace MiniStore.Infrastructure.Repositories;

public sealed class InventoryBalanceRepository(AppDbContext context) : IInventoryBalanceRepository
{
    public Task<List<InventoryBalance>> GetAllAsync() => context.InventoryBalances.AsNoTracking()
        .OrderBy(x => x.WarehouseId).ThenBy(x => x.ProductId).ThenBy(x => x.StorageLocationId).ToListAsync();
    public Task<decimal> GetWarehouseAvailableAsync(int productId, int warehouseId) =>
        context.InventoryBalances.Where(x => x.ProductId == productId && x.WarehouseId == warehouseId)
            .SumAsync(x => x.OnHand - x.Reserved);
}
