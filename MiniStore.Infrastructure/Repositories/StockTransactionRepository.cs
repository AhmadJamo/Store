using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;

namespace MiniStore.Infrastructure.Repositories;

public class StockTransactionRepository
    : IStockTransactionRepository
{
    private readonly AppDbContext _context;

    public StockTransactionRepository(
        AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<StockTransaction>> GetAllAsync()
    {
        return await _context.StockTransactions
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<StockTransaction>>
        GetByProductAndWarehouseAsync(
            int productId,
            int warehouseId)
    {
        return await _context.StockTransactions
            .Where(x =>
                x.ProductId == productId &&
                x.WarehouseId == warehouseId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task AddAsync(
        StockTransaction transaction)
    {
        await _context.StockTransactions
            .AddAsync(transaction);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}