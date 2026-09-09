using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;

namespace MiniStore.Infrastructure.Repositories;

public class SaleRepository : ISaleRepository
{
    private readonly AppDbContext _context;

    public SaleRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Sale>> GetAllAsync()
    {
        return await _context.Sales
            .Include(x => x.Items)
            .OrderByDescending(x => x.Date)
            .ToListAsync();
    }

    public async Task<Sale?> GetByIdAsync(int id)
    {
        return await _context.Sales
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task AddAsync(Sale sale)
    {
        await _context.Sales.AddAsync(sale);
    }
}