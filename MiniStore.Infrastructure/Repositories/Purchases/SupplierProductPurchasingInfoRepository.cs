using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;

namespace MiniStore.Infrastructure.Repositories;

public sealed class SupplierProductPurchasingInfoRepository(AppDbContext context)
    : ISupplierProductPurchasingInfoRepository
{
    public Task<List<SupplierProductPurchasingInfo>> GetAllAsync() => context.SupplierProductPurchasingInfos
        .Include(x => x.PriceBreaks)
        .OrderBy(x => x.ProductId).ThenBy(x => x.Priority).ThenBy(x => x.SupplierId)
        .ToListAsync();

    public Task<List<SupplierProductPurchasingInfo>> GetBySupplierIdAsync(int supplierId) =>
        context.SupplierProductPurchasingInfos
            .Include(x => x.PriceBreaks)
            .Where(x => x.SupplierId == supplierId)
            .OrderBy(x => x.ProductId).ThenBy(x => x.Priority)
            .ToListAsync();

    public Task<List<SupplierProductPurchasingInfo>> GetByProductIdAsync(int productId) =>
        context.SupplierProductPurchasingInfos
            .Include(x => x.PriceBreaks)
            .Where(x => x.ProductId == productId)
            .OrderBy(x => x.Priority).ThenBy(x => x.SupplierId)
            .ToListAsync();

    public Task<SupplierProductPurchasingInfo?> GetByIdAsync(int id) =>
        context.SupplierProductPurchasingInfos
            .Include(x => x.PriceBreaks)
            .FirstOrDefaultAsync(x => x.Id == id);

    public Task<List<SupplierProductPriceHistory>> GetPriceHistoryAsync(int purchasingInfoId) =>
        context.SupplierProductPriceHistories.AsNoTracking()
            .Include(x => x.PriceBreaks)
            .Where(x => x.SupplierProductPurchasingInfoId == purchasingInfoId)
            .OrderByDescending(x => x.Revision)
            .ToListAsync();

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
