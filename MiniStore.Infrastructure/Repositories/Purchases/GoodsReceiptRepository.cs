using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;

namespace MiniStore.Infrastructure.Repositories;

public sealed class GoodsReceiptRepository(AppDbContext context) : IGoodsReceiptRepository
{
    public Task<List<GoodsReceipt>> GetAllAsync(string? search)
    {
        var query = context.GoodsReceipts.Include(x => x.Lines).AsQueryable();
        if (!string.IsNullOrWhiteSpace(search)) query = query.Where(x => x.ReceiptNumber.Contains(search));
        return query.OrderByDescending(x => x.CreatedAtUtc).ToListAsync();
    }
    public Task<List<GoodsReceipt>> GetByPurchaseOrderIdAsync(int purchaseOrderId) => context.GoodsReceipts.Include(x => x.Lines)
        .Where(x => x.PurchaseOrderId == purchaseOrderId).OrderByDescending(x => x.CreatedAtUtc).ToListAsync();
    public Task<GoodsReceipt?> GetByIdAsync(int id) => context.GoodsReceipts.Include(x => x.Lines).FirstOrDefaultAsync(x => x.Id == id);
    public Task AddAsync(GoodsReceipt receipt) => context.GoodsReceipts.AddAsync(receipt).AsTask();
}
