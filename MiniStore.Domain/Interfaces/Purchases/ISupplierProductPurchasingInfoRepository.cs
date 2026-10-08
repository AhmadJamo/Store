using MiniStore.Domain.Entities;

namespace MiniStore.Domain.Interfaces;

public interface ISupplierProductPurchasingInfoRepository
{
    Task<List<SupplierProductPurchasingInfo>> GetAllAsync();
    Task<SupplierProductPurchasingInfo?> GetByIdAsync(int id);
    Task<SupplierProductPurchasingInfo?> FindDuplicateAsync(
        int supplierId, int productId, int purchaseMeasurementUnitId, int? excludingId = null);
    Task<List<SupplierProductPurchasingInfo>> GetPreferredForProductAsync(int productId, int? excludingId = null);
    Task AddAsync(SupplierProductPurchasingInfo info);
    Task SaveChangesAsync();
}
