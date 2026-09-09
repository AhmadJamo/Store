using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;

namespace MiniStore.Infrastructure.Repositories;

public class SupplierRepository : ISupplierRepository
{
    private readonly AppDbContext _context;

    public SupplierRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Supplier?> GetByIdAsync(int id)
    {
        return await _context.Suppliers
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<Supplier>> GetAllAsync()
    {
        return await _context.Suppliers
            .OrderBy(x => x.Name)
            .ToListAsync();
    }

    public async Task AddAsync(Supplier supplier)
    {
        await _context.Suppliers.AddAsync(supplier);
    }

    public async Task DeleteAsync(Supplier supplier)
    {
        _context.Suppliers.Remove(supplier);

        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}