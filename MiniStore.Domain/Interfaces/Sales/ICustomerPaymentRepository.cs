using MiniStore.Domain.Entities;

namespace MiniStore.Domain.Interfaces;

public interface ICustomerPaymentRepository
{
    Task<List<CustomerPayment>> GetAllAsync(string? search = null);
    Task<CustomerPayment?> GetByIdAsync(int id);
    Task<Dictionary<int, decimal>> GetPaidAmountsAsync(IEnumerable<int> customerInvoiceIds);
    Task<Dictionary<int, decimal>> GetReceivableCarryingAmountsAsync(IEnumerable<int> customerInvoiceIds);
    Task AddAsync(CustomerPayment payment);
}
