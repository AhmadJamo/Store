using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

/// <summary>
/// Maintains the single supplier-product relationship used from both supplier and product screens.
/// Existing purchasing terms are deactivated, not deleted, so re-linking restores their details.
/// </summary>
public sealed class SupplierProductAssignmentService(
    ISupplierProductPurchasingInfoRepository purchasingInfos,
    ISupplierRepository suppliers,
    IProductRepository products,
    IMeasurementUnitRepository units,
    IGeneralSettingsRepository settings,
    ICurrentUserService currentUser)
{
    public async Task<List<int>> GetProductIdsForSupplierAsync(int supplierId) =>
        (await purchasingInfos.GetBySupplierIdAsync(supplierId))
        .Where(x => x.IsActive).Select(x => x.ProductId).Distinct().ToList();

    public async Task<List<int>> GetSupplierIdsForProductAsync(int productId) =>
        (await purchasingInfos.GetByProductIdAsync(productId))
        .Where(x => x.IsActive).Select(x => x.SupplierId).Distinct().ToList();

    public async Task ApplyForSupplierAsync(int supplierId, IEnumerable<int>? selectedProductIds)
    {
        if (await suppliers.GetByIdAsync(supplierId) is null)
            throw new InvalidOperationException("Supplier not found.");

        var selected = Normalize(selectedProductIds);
        var existing = await purchasingInfos.GetBySupplierIdAsync(supplierId);
        foreach (var group in existing.GroupBy(x => x.ProductId))
            SetGroupActive(group, selected.Contains(group.Key));

        var existingIds = existing.Select(x => x.ProductId).ToHashSet();
        foreach (var productId in selected.Except(existingIds))
            await CreateDefaultAsync(supplierId, productId);
    }

    public async Task ApplyForProductAsync(int productId, IEnumerable<int>? selectedSupplierIds)
    {
        var selected = Normalize(selectedSupplierIds);
        var product = await products.GetByIdAsync(productId)
            ?? throw new ArgumentException("Selected product was not found.");
        if ((!product.IsActive || product.ProductType == ProductType.PreparedToOrder) && selected.Count > 0)
            throw new InvalidOperationException(product.IsActive
                ? "Prepared-to-order products cannot be assigned to suppliers."
                : "Selected product is inactive.");
        foreach (var supplierId in selected)
        {
            var supplier = await suppliers.GetByIdAsync(supplierId)
                ?? throw new ArgumentException("Selected supplier was not found.");
            if (!supplier.IsActive)
                throw new InvalidOperationException("Selected supplier is inactive.");
        }

        var existing = await purchasingInfos.GetByProductIdAsync(productId);
        foreach (var group in existing.GroupBy(x => x.SupplierId))
            SetGroupActive(group, selected.Contains(group.Key));

        if (selected.Count == 0) return;

        var existingIds = existing.Select(x => x.SupplierId).ToHashSet();
        foreach (var supplierId in selected.Except(existingIds))
            await CreateDefaultAsync(supplierId, product);
    }

    private async Task CreateDefaultAsync(int supplierId, int productId) =>
        await CreateDefaultAsync(supplierId, await GetEligibleProductAsync(productId));

    private async Task CreateDefaultAsync(int supplierId, Product product)
    {
        var supplier = await suppliers.GetByIdAsync(supplierId)
            ?? throw new ArgumentException("Selected supplier was not found.");
        if (!supplier.IsActive)
            throw new InvalidOperationException("Selected supplier is inactive.");
        if (!product.MeasurementUnitId.HasValue ||
            await units.GetByIdAsync(product.MeasurementUnitId.Value) is not { IsActive: true })
            throw new InvalidOperationException("The product must have an active managed stock unit before assigning a supplier.");

        var baseCurrency = (await settings.GetAsync())?.Currency?.Trim().ToUpperInvariant();
        var info = new SupplierProductPurchasingInfo(
            supplierId, product.Id, product.MeasurementUnitId.Value,
            string.IsNullOrWhiteSpace(product.ProductCode) ? $"P-{product.Id}" : product.ProductCode,
            null, 1m, 1m, 0, product.PurchasePrice,
            string.IsNullOrWhiteSpace(baseCurrency) ? "JOD" : baseCurrency,
            DateOnly.FromDateTime(DateTime.Today), null, false, 100);
        info.RecordPriceSnapshot(SupplierPriceChangeSource.Assigned, currentUser.UserId);
        await purchasingInfos.AddAsync(info);
    }

    private async Task<Product> GetEligibleProductAsync(int productId)
    {
        var product = await products.GetByIdAsync(productId)
            ?? throw new ArgumentException("Selected product was not found.");
        if (!product.IsActive)
            throw new InvalidOperationException("Selected product is inactive.");
        if (product.ProductType == ProductType.PreparedToOrder)
            throw new InvalidOperationException("Prepared-to-order products cannot be assigned to suppliers.");
        return product;
    }

    private static HashSet<int> Normalize(IEnumerable<int>? ids) =>
        (ids ?? []).Where(x => x > 0).ToHashSet();

    private static void SetGroupActive(IEnumerable<SupplierProductPurchasingInfo> group, bool active)
    {
        foreach (var info in group)
            info.SetActive(active);
    }
}
