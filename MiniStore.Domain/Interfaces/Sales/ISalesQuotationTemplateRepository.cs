using MiniStore.Domain.Entities;

namespace MiniStore.Domain.Interfaces;

public interface ISalesQuotationTemplateRepository
{
    Task<List<SalesQuotationTemplate>> GetAllAsync(SalesQuotationTemplateStatus? status, string? search);
    Task<SalesQuotationTemplate?> GetByIdAsync(int id, bool tracking = true);
    Task<SalesQuotationTemplate?> GetDefaultAsync();
    Task<SalesQuotationTemplate?> GetByCodeVersionAsync(string code, int version);
    Task<List<SalesQuotationTemplate>> GetActiveByCodeAsync(string code);
    Task<List<SalesQuotationTemplate>> GetActiveDefaultsAsync();
    Task AddAsync(SalesQuotationTemplate template);
}
