using MiniStore.Domain.Entities;
namespace MiniStore.Domain.Interfaces;
public interface ICustomerCreditNoteRepository
{
    Task<List<CustomerCreditNote>> GetAllAsync();
    Task<List<CustomerCreditNote>> GetByInvoiceAsync(int invoiceId);
    Task<CustomerCreditNote?> GetByIdAsync(int id);
    Task<CustomerCreditNote?> GetByRequestKeyAsync(string requestKey);
    Task<Dictionary<int,decimal>> GetPostedAmountsAsync(IEnumerable<int> invoiceIds);
    Task<Dictionary<int,decimal>> GetPostedCompanyAmountsAsync(IEnumerable<int> invoiceIds);
    Task AddAsync(CustomerCreditNote note);
}
