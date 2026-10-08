using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;

namespace MiniStore.Infrastructure.Repositories;

public sealed class PurchaseSourcingRepository(AppDbContext context) : IPurchaseSourcingRepository
{
    public Task<List<PurchaseSourcingEvent>> GetAllAsync(PurchaseSourcingStatus? status, string? search)
    {
        var query = context.PurchaseSourcingEvents.Include(x => x.Invitations).AsQueryable();
        if (status.HasValue) query = query.Where(x => x.Status == status);
        if (!string.IsNullOrWhiteSpace(search)) query = query.Where(x => x.SourcingNumber.Contains(search));
        return query.OrderByDescending(x => x.CreatedAtUtc).ToListAsync();
    }

    public Task<PurchaseSourcingEvent?> GetByIdAsync(int id) => context.PurchaseSourcingEvents
        .Include(x => x.Lines).Include(x => x.Invitations).FirstOrDefaultAsync(x => x.Id == id);

    public Task<PurchaseSourcingEvent?> GetByPurchaseRequestIdAsync(int purchaseRequestId) =>
        context.PurchaseSourcingEvents.SingleOrDefaultAsync(x => x.PurchaseRequestId == purchaseRequestId);

    public Task AddAsync(PurchaseSourcingEvent sourcingEvent) => context.PurchaseSourcingEvents.AddAsync(sourcingEvent).AsTask();
}
