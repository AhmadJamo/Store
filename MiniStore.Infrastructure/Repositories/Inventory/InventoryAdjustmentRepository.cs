using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;
namespace MiniStore.Infrastructure.Repositories;
public sealed class InventoryAdjustmentRepository(AppDbContext db) : IInventoryAdjustmentRepository
{
    public Task<List<InventoryAdjustment>> GetAllAsync() => db.InventoryAdjustments.AsNoTracking().Include(x => x.Lines).OrderByDescending(x => x.CreatedAt).ToListAsync();
    public Task<InventoryAdjustment?> GetByIdAsync(long id) => db.InventoryAdjustments.Include(x => x.Lines).SingleOrDefaultAsync(x => x.Id == id);
    public Task<InventoryBalance?> GetBalanceAsync(int productId, int warehouseId, int? locationId) => db.InventoryBalances.SingleOrDefaultAsync(x => x.ProductId == productId && x.WarehouseId == warehouseId && x.StorageLocationId == locationId);
    public Task AddAsync(InventoryAdjustment adjustment) => db.InventoryAdjustments.AddAsync(adjustment).AsTask();
}
