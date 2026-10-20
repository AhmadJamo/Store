using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;

namespace MiniStore.Infrastructure.Repositories;

public sealed class SalesDeliveryRepository(AppDbContext context) : ISalesDeliveryRepository
{
    public Task<List<SalesDelivery>> GetAllAsync(string? search = null)
    {
        var query = context.SalesDeliveries.AsNoTracking().Include(x => x.Lines).AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(x => x.DeliveryNumber.Contains(term));
        }
        return query.OrderByDescending(x => x.CreatedAtUtc).ToListAsync();
    }
    public Task<List<SalesDelivery>> GetBySalesOrderAsync(int salesOrderId) => context.SalesDeliveries
        .AsNoTracking().Include(x => x.Lines).Where(x => x.SalesOrderId == salesOrderId)
        .OrderByDescending(x => x.CreatedAtUtc).ToListAsync();
    public Task<SalesDelivery?> GetByIdAsync(int id) => context.SalesDeliveries
        .Include(x => x.Lines).Include(x => x.History).SingleOrDefaultAsync(x => x.Id == id);
    public Task<SalesDelivery?> GetDraftByReservationAsync(long reservationId) => context.SalesDeliveries
        .Include(x => x.Lines).SingleOrDefaultAsync(x => x.InventoryReservationId == reservationId && x.Status == SalesDeliveryStatus.Draft);
    public Task AddAsync(SalesDelivery delivery) => context.SalesDeliveries.AddAsync(delivery).AsTask();
}
