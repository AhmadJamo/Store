using MiniStore.Domain.Entities;

namespace MiniStore.Domain.Interfaces;

public interface ICustomerReturnRequestRepository
{
    Task<List<CustomerReturnRequest>> GetAllAsync(string? search = null);
    Task<CustomerReturnRequest?> GetByIdAsync(int id);
    Task<Dictionary<int, decimal>> GetActiveRequestedQuantitiesAsync(int salesDeliveryId, int? excludingRequestId = null);
    Task AddAsync(CustomerReturnRequest request);
}
