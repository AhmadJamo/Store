using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;

namespace MiniStore.Infrastructure.Repositories;

public sealed class CustomerReturnRequestRepository(AppDbContext context) : ICustomerReturnRequestRepository
{
    public Task<List<CustomerReturnRequest>> GetAllAsync(string? search = null)
    {
        var query = context.CustomerReturnRequests.Include(x => x.Lines).AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(search)) { var term = search.Trim(); query = query.Where(x => x.ReturnNumber.Contains(term) || x.OrderNumberSnapshot.Contains(term) || x.CustomerNameSnapshot.Contains(term)); }
        return query.OrderByDescending(x => x.RequestDate).ThenByDescending(x => x.Id).ToListAsync();
    }
    public Task<CustomerReturnRequest?> GetByIdAsync(int id) => context.CustomerReturnRequests.Include(x => x.Lines).Include(x => x.History).SingleOrDefaultAsync(x => x.Id == id);
    public Task<Dictionary<int, decimal>> GetActiveRequestedQuantitiesAsync(int deliveryId, int? excludingRequestId = null)
    {
        var active = new[] { CustomerReturnRequestStatus.Draft, CustomerReturnRequestStatus.Submitted, CustomerReturnRequestStatus.Approved };
        return context.CustomerReturnRequestLines.Join(context.CustomerReturnRequests.Where(x => x.SalesDeliveryId == deliveryId && active.Contains(x.Status) && (!excludingRequestId.HasValue || x.Id != excludingRequestId.Value)),
            line => line.CustomerReturnRequestId, request => request.Id, (line, request) => line)
            .GroupBy(x => x.SalesDeliveryLineId).Select(x => new { LineId = x.Key, Quantity = x.Sum(y => y.StockQuantity) })
            .ToDictionaryAsync(x => x.LineId, x => x.Quantity);
    }
    public Task AddAsync(CustomerReturnRequest request) => context.CustomerReturnRequests.AddAsync(request).AsTask();
}
