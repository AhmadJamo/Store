using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;

namespace MiniStore.Infrastructure.Repositories;

public class PurchaseRepository : IPurchaseRepository
{
    private readonly AppDbContext _context;

    public PurchaseRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Purchase?> GetByIdAsync(int id)
    {
        return await _context.Purchases
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<Purchase>> GetAllAsync()
    {
        return await _context.Purchases
            .Include(x => x.Items)
            .OrderByDescending(x => x.Date)
            .ThenByDescending(x => x.Id)
            .ToListAsync();
    }

    public async Task AddAsync(Purchase purchase)
    {
        await _context.Purchases.AddAsync(purchase);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}