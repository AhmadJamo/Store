using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;
namespace MiniStore.Infrastructure.Repositories;
public class ProductLocationStockRepository(AppDbContext context) : IProductLocationStockRepository
{
    public Task<List<ProductLocationStock>> GetAllAsync() => context.ProductLocationStocks.AsNoTracking().ToListAsync();
    public Task<ProductLocationStock?> GetAsync(int productId, int storageLocationId) => context.ProductLocationStocks.FirstOrDefaultAsync(x => x.ProductId == productId && x.StorageLocationId == storageLocationId);
    public Task AddAsync(ProductLocationStock stock) => context.ProductLocationStocks.AddAsync(stock).AsTask();
    public async Task RemoveAllAsync(int productId, int warehouseId)
    {
        var allocations = await context.ProductLocationStocks
            .Where(x => x.ProductId == productId && x.WarehouseId == warehouseId)
            .ToListAsync();

        context.ProductLocationStocks.RemoveRange(allocations);
    }
}
