using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;

namespace MiniStore.Infrastructure.Repositories;

public class SalesReturnRepository(AppDbContext context) : ISalesReturnRepository
{
    public Task<List<SalesReturn>> GetAllAsync() => context.SalesReturns
        .Include(x => x.Items)
        .OrderByDescending(x => x.Date)
        .ThenByDescending(x => x.Id)
        .ToListAsync();

    public Task<SalesReturn?> GetByIdAsync(int id) => context.SalesReturns
        .Include(x => x.Items)
        .FirstOrDefaultAsync(x => x.Id == id);

    public Task<List<SalesReturn>> GetBySaleIdAsync(int saleId) => context.SalesReturns
        .Include(x => x.Items)
        .Where(x => x.SaleId == saleId)
        .ToListAsync();

    public Task AddAsync(SalesReturn salesReturn) =>
        context.SalesReturns.AddAsync(salesReturn).AsTask();
}
