using MiniStore.Domain.Entities;

namespace MiniStore.Domain.Interfaces;

public interface ISalesQuotationRepository
{
    Task<List<SalesQuotation>> GetAllAsync(SalesQuotationStatus? status, string? search);
    Task<SalesQuotation?> GetByIdAsync(int id);
    Task<SalesQuotation?> GetByRequestKeyAsync(string requestKey);
    Task AddAsync(SalesQuotation quotation);
}
