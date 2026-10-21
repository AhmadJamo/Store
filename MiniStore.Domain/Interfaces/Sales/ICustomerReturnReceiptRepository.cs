using MiniStore.Domain.Entities;
namespace MiniStore.Domain.Interfaces;
public interface ICustomerReturnReceiptRepository
{
    Task<List<CustomerReturnReceipt>> GetAllAsync(string? search = null);
    Task<List<CustomerReturnReceipt>> GetByRequestAsync(int requestId);
    Task<CustomerReturnReceipt?> GetByIdAsync(int id);
    Task<Dictionary<int, decimal>> GetActiveReceivedQuantitiesAsync(int requestId, int? excludingReceiptId = null);
    Task AddAsync(CustomerReturnReceipt receipt);
}
