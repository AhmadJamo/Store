using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;

namespace MiniStore.Infrastructure.Repositories;

public sealed class GoodsReceiptReturnRepository(AppDbContext context) : IGoodsReceiptReturnRepository
{
    public Task<List<GoodsReceiptReturn>> GetAllAsync() => context.GoodsReceiptReturns.Include(x => x.Lines).OrderByDescending(x => x.CreatedAtUtc).ToListAsync();
    public Task<List<GoodsReceiptReturn>> GetByGoodsReceiptIdAsync(int goodsReceiptId) => context.GoodsReceiptReturns.Include(x => x.Lines).Where(x => x.GoodsReceiptId == goodsReceiptId).ToListAsync();
    public Task<GoodsReceiptReturn?> GetByIdAsync(int id) => context.GoodsReceiptReturns.Include(x => x.Lines).SingleOrDefaultAsync(x => x.Id == id);
    public Task AddAsync(GoodsReceiptReturn value) => context.GoodsReceiptReturns.AddAsync(value).AsTask();
}
