using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;

namespace MiniStore.Infrastructure.Repositories;

public class ProductStockRepository : IProductStockRepository
{
    private readonly AppDbContext _context;

    public ProductStockRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ProductStock?> GetByIdAsync(int id)
    {
        return await _context.ProductStocks
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<ProductStock?> GetByProductAndWarehouseAsync(
        int productId,
        int warehouseId)
    {
        return await _context.ProductStocks
            .FirstOrDefaultAsync(x =>
                x.ProductId == productId &&
                x.WarehouseId == warehouseId);
    }

    public async Task<List<ProductStock>> GetAllAsync()
    {
        return await _context.ProductStocks
            .ToListAsync();
    }

    public async Task AddAsync(ProductStock productStock)
    {
        await _context.ProductStocks.AddAsync(productStock);
    }

    public async Task DeleteAsync(ProductStock productStock)
    {
        _context.ProductStocks.Remove(productStock);

        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}