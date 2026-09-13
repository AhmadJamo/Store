using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;

namespace MiniStore.Infrastructure.Repositories;

public class WarehouseRepository : IWarehouseRepository
{
    private readonly AppDbContext _context;

    public WarehouseRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Warehouse?> GetByIdAsync(int id)
    {
        return await _context.Warehouses
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<Warehouse>> GetAllAsync()
    {
        return await _context.Warehouses
            .ToListAsync();
    }

    public async Task AddAsync(Warehouse warehouse)
    {
        await _context.Warehouses.AddAsync(warehouse);
    }

    public async Task DeleteAsync(Warehouse warehouse)
    {
        _context.Warehouses.Remove(warehouse);

        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}