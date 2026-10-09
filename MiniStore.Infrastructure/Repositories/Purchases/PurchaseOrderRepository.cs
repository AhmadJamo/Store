using Microsoft.EntityFrameworkCore;using MiniStore.Domain.Entities;using MiniStore.Domain.Interfaces;using MiniStore.Infrastructure.Persistence;
namespace MiniStore.Infrastructure.Repositories;
public sealed class PurchaseOrderRepository(AppDbContext context) : IPurchaseOrderRepository
{
    public Task<List<PurchaseOrder>> GetAllAsync(PurchaseOrderStatus? status, string? search)
    {
        var query = context.PurchaseOrders.AsQueryable();
        if (status.HasValue) query = query.Where(x => x.Status == status);
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(x => x.OrderNumber.Contains(search) || x.SupplierQuotationId.ToString().Contains(search));
        return query.OrderByDescending(x => x.CreatedAtUtc).ToListAsync();
    }

    public Task<PurchaseOrder?> GetByIdAsync(int id) => context.PurchaseOrders.Include(x => x.Lines).FirstOrDefaultAsync(x => x.Id == id);
    public Task<PurchaseOrder?> GetByQuotationIdAsync(int quotationId) => context.PurchaseOrders.FirstOrDefaultAsync(x => x.SupplierQuotationId == quotationId);
    public Task AddAsync(PurchaseOrder order) => context.PurchaseOrders.AddAsync(order).AsTask();
}
