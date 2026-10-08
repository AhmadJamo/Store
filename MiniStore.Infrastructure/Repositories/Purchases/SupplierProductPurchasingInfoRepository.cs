using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;

namespace MiniStore.Infrastructure.Repositories;

public sealed class SupplierProductPurchasingInfoRepository(AppDbContext context)
    : ISupplierProductPurchasingInfoRepository
{
    public Task<List<SupplierProductPurchasingInfo>> GetAllAsync() => context.SupplierProductPurchasingInfos
        .OrderBy(x => x.ProductId).ThenBy(x => x.Priority).ThenBy(x => x.SupplierId)
        .ToListAsync();

    public Task<SupplierProductPurchasingInfo?> GetByIdAsync(int id) =>
        context.SupplierProductPurchasingInfos.FirstOrDefaultAsync(x => x.Id == id);

    public Task<SupplierProductPurchasingInfo?> FindDuplicateAsync(
        int supplierId, int productId, int purchaseMeasurementUnitId, int? excludingId = null) =>
        context.SupplierProductPurchasingInfos.FirstOrDefaultAsync(x =>
            x.SupplierId == supplierId && x.ProductId == productId &&
            x.PurchaseMeasurementUnitId == purchaseMeasurementUnitId &&
            (!excludingId.HasValue || x.Id != excludingId.Value));

    public Task<List<SupplierProductPurchasingInfo>> GetPreferredForProductAsync(
        int productId, int? excludingId = null) => context.SupplierProductPurchasingInfos
        .Where(x => x.ProductId == productId && x.IsPreferred &&
                    (!excludingId.HasValue || x.Id != excludingId.Value))
        .ToListAsync();

    public Task AddAsync(SupplierProductPurchasingInfo info) =>
        context.SupplierProductPurchasingInfos.AddAsync(info).AsTask();

    public Task SaveChangesAsync() => context.SaveChangesAsync();
}
