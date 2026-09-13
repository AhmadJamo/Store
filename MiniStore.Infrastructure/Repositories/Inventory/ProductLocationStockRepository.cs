using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;

namespace MiniStore.Infrastructure.Repositories;

public class ProductLocationStockRepository(AppDbContext context) : IProductLocationStockRepository
{
    public Task<List<ProductLocationStock>> GetAllAsync()
    {
        return context.ProductLocationStocks
            .AsNoTracking()
            .ToListAsync();
    }

    public Task<ProductLocationStock?> GetAsync(
        int productId,
        int storageLocationId)
    {
        return context.ProductLocationStocks.FirstOrDefaultAsync(stock =>
            stock.ProductId == productId &&
            stock.StorageLocationId == storageLocationId);
    }

    public Task AddAsync(ProductLocationStock stock)
    {
        return context.ProductLocationStocks.AddAsync(stock).AsTask();
    }

    public async Task RemoveAllAsync(int productId, int warehouseId)
    {
        var allocations = await context.ProductLocationStocks
            .Where(stock =>
                stock.ProductId == productId &&
                stock.WarehouseId == warehouseId)
            .ToListAsync();

        context.ProductLocationStocks.RemoveRange(allocations);
    }
}
