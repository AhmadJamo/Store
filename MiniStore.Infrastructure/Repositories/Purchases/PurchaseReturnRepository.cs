using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;
namespace MiniStore.Infrastructure.Repositories;
public sealed class PurchaseReturnRepository(AppDbContext context) : IPurchaseReturnRepository
{
    public Task<List<PurchaseReturn>> GetAllAsync() => context.PurchaseReturns.Include(x => x.Items).OrderByDescending(x => x.Date).ThenByDescending(x => x.Id).ToListAsync();
    public Task<PurchaseReturn?> GetByIdAsync(int id) => context.PurchaseReturns.Include(x => x.Items).FirstOrDefaultAsync(x => x.Id == id);
    public Task<List<PurchaseReturn>> GetByPurchaseIdAsync(int purchaseId) => context.PurchaseReturns.Include(x => x.Items).Where(x => x.PurchaseId == purchaseId).ToListAsync();
    public Task AddAsync(PurchaseReturn purchaseReturn) => context.PurchaseReturns.AddAsync(purchaseReturn).AsTask();
}
