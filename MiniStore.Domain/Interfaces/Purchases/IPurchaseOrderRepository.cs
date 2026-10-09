using MiniStore.Domain.Entities;
namespace MiniStore.Domain.Interfaces;
public interface IPurchaseOrderRepository
{
    Task<List<PurchaseOrder>> GetAllAsync(PurchaseOrderStatus? status, string? search);
    Task<PurchaseOrder?> GetByIdAsync(int id);
    Task<PurchaseOrder?> GetByQuotationIdAsync(int quotationId);
    Task AddAsync(PurchaseOrder order);
}
