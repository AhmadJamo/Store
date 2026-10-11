using MiniStore.Domain.Entities;
namespace MiniStore.Domain.Interfaces;
public interface IPurchaseAgreementRepository
{
    Task<List<PurchaseAgreement>> GetAllAsync();
    Task<PurchaseAgreement?> GetByIdAsync(int id);
    Task<PurchaseAgreement?> GetByQuotationIdAsync(int quotationId);
    Task AddAsync(PurchaseAgreement agreement);
}
public interface IPurchaseTemplateRepository
{
    Task<List<PurchaseTemplate>> GetAllAsync();
    Task<PurchaseTemplate?> GetByIdAsync(int id);
    Task AddAsync(PurchaseTemplate template);
}
