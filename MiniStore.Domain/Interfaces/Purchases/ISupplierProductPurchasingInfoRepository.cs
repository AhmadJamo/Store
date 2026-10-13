using MiniStore.Domain.Entities;

namespace MiniStore.Domain.Interfaces;

public interface ISupplierProductPurchasingInfoRepository
{
    Task<List<SupplierProductPurchasingInfo>> GetAllAsync();
    Task<List<SupplierProductPurchasingInfo>> GetBySupplierIdAsync(int supplierId);
    Task<List<SupplierProductPurchasingInfo>> GetByProductIdAsync(int productId);
    Task<SupplierProductPurchasingInfo?> GetByIdAsync(int id);
    Task<List<SupplierProductPriceHistory>> GetPriceHistoryAsync(int purchasingInfoId);
    Task<SupplierProductPurchasingInfo?> FindDuplicateAsync(
        int supplierId, int productId, int purchaseMeasurementUnitId, int? excludingId = null);
    Task<List<SupplierProductPurchasingInfo>> GetPreferredForProductAsync(int productId, int? excludingId = null);
    Task AddAsync(SupplierProductPurchasingInfo info);
    Task SaveChangesAsync();
}
