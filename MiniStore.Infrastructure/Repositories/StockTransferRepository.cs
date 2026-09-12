using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;

namespace MiniStore.Infrastructure.Repositories;

public class StockTransferRepository : IStockTransferRepository
{
    private readonly AppDbContext _context;

    public StockTransferRepository(
        AppDbContext context)
    {
        _context = context;
    }

    public async Task<StockTransfer?> GetByIdAsync(
        int id)
    {
        return await _context.StockTransfers
            .Include(x => x.Items)
            .Include(x => x.History)
            .FirstOrDefaultAsync(
                x => x.Id == id);
    }

    public async Task<StockTransfer?> GetByTransferNumberAsync(
        string transferNumber)
    {
        return await _context.StockTransfers
            .Include(x => x.Items)
            .Include(x => x.History)
            .FirstOrDefaultAsync(
                x => x.TransferNumber == transferNumber);
    }

    public async Task<List<StockTransfer>> GetAllAsync(
      string? search,
      StockTransferStatus? status)
    {
        var query = _context.StockTransfers
            .Include(x => x.Items)
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();

            query = query.Where(x =>
                x.TransferNumber.Contains(search) ||
                (x.Reference != null &&
                 x.Reference.Contains(search)));
        }

        if (status.HasValue)
        {
            query = query.Where(
                x => x.Status == status.Value);
        }

        return await query
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task AddAsync(
        StockTransfer transfer)
    {
        await _context.StockTransfers.AddAsync(
            transfer);
    }


    public async Task DeleteItemAsync(
    StockTransferItem item)
    {
        _context.StockTransferItems.Remove(item);

        await Task.CompletedTask;
    }


    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}