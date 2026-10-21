using MiniStore.Domain.Entities;

namespace MiniStore.Domain.Interfaces;

public interface ICustomerInvoiceRepository
{
    Task<List<CustomerInvoice>> GetAllAsync(CustomerInvoiceStatus? status = null, string? search = null);
    Task<List<CustomerInvoice>> GetBySalesOrderAsync(int salesOrderId);
    Task<List<CustomerInvoice>> GetByCustomerAsync(int customerId);
    Task<CustomerInvoice?> GetByIdAsync(int id);
    Task<CustomerInvoice?> GetByRequestKeyAsync(string requestKey);
    Task AddAsync(CustomerInvoice invoice);
}
