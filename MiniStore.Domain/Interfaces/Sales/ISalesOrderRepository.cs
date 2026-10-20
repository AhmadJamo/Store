using MiniStore.Domain.Entities;

namespace MiniStore.Domain.Interfaces;

public interface ISalesOrderRepository
{
    Task<List<SalesOrder>> GetAllAsync(SalesOrderStatus? status, string? search);
    Task<SalesOrder?> GetByIdAsync(int id);
    Task<SalesOrder?> GetBySourceQuotationIdAsync(int quotationId);
    Task AddAsync(SalesOrder order);
}
