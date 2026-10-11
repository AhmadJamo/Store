using MiniStore.Application.DTOs.Purchases;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public sealed class SupplierPurchasingInfoService(
    ISupplierProductPurchasingInfoRepository repository,
    ISupplierRepository supplierRepository,
    IProductRepository productRepository,
    IMeasurementUnitRepository unitRepository,
    IGeneralSettingsRepository generalSettingsRepository,
    IUnitOfWork unitOfWork)
{
    public async Task<SupplierPurchasingInfoPageDto> GetPageAsync(int? supplierId = null)
    {
        var infos = await repository.GetAllAsync();
        var suppliers = await supplierRepository.GetAllAsync();
        var products = await productRepository.GetAllAsync(null);
        var units = await unitRepository.GetAllAsync();
        var settings = await generalSettingsRepository.GetAsync();
        var supplierNames = suppliers.ToDictionary(x => x.Id, x => x.Name);
        var productsById = products.ToDictionary(x => x.Id);
        var unitsById = units.ToDictionary(x => x.Id);

        var baseCurrency = NormalizeBaseCurrency(settings?.Currency);
        if (supplierId.HasValue && suppliers.All(x => x.Id != supplierId.Value))
            throw new InvalidOperationException("Supplier was not found.");
        var selectedInfos = supplierId.HasValue
            ? infos.Where(x => x.SupplierId == supplierId.Value).ToList()
            : [];
        var bulkRows = supplierId.HasValue
            ? BuildBulkRows(products, units, selectedInfos, baseCurrency)
            : [];

        return new SupplierPurchasingInfoPageDto
        {
            Rows = selectedInfos.Select(info => new SupplierPurchasingInfoDto
            {
                Id = info.Id,
                SupplierId = info.SupplierId,
                SupplierName = supplierNames.GetValueOrDefault(info.SupplierId, $"#{info.SupplierId}"),
                ProductId = info.ProductId,
                ProductName = productsById.GetValueOrDefault(info.ProductId)?.Name ?? $"#{info.ProductId}",
                ProductCode = productsById.GetValueOrDefault(info.ProductId)?.ProductCode ?? string.Empty,
                PurchaseMeasurementUnitId = info.PurchaseMeasurementUnitId,
                PurchaseUnit = unitsById.TryGetValue(info.PurchaseMeasurementUnitId, out var unit)
                    ? $"{unit.Name} ({unit.Symbol})"
                    : $"#{info.PurchaseMeasurementUnitId}",
                SupplierProductCode = info.SupplierProductCode,
                SupplierDescription = info.SupplierDescription,
                MinimumOrderQuantity = info.MinimumOrderQuantity,
                OrderMultiple = info.OrderMultiple,
                LeadTimeDays = info.LeadTimeDays,
                UnitPrice = info.UnitPrice,
                CurrencyCode = info.CurrencyCode,
                ValidFrom = info.ValidFrom,
                ValidTo = info.ValidTo,
                IsPreferred = info.IsPreferred,
                Priority = info.Priority,
                IsActive = info.IsActive,
                RowVersion = info.RowVersion
            }).ToList(),
            Suppliers = suppliers.Select(x => new SupplierPurchasingOptionDto(x.Id, x.Name)).ToList(),
            Products = products.Where(x => x.IsActive && x.ProductType != ProductType.PreparedToOrder)
                .OrderBy(x => x.Name)
                .Select(x => new SupplierPurchasingOptionDto(x.Id, $"{x.ProductCode} — {x.Name}"))
                .ToList(),
            Units = units.Where(x => x.IsActive).OrderBy(x => x.Dimension).ThenBy(x => x.Name)
                .Select(x => new SupplierPurchasingOptionDto(x.Id, $"{x.Name} ({x.Symbol}) — {x.Dimension}"))
                .ToList(),
            Form = new SaveSupplierPurchasingInfoDto { SupplierId = supplierId ?? 0, CurrencyCode = baseCurrency },
            SelectedSupplierId = supplierId,
            BulkForm = new BulkSaveSupplierPurchasingInfoDto { SupplierId = supplierId ?? 0, Rows = bulkRows },
            BaseCurrency = baseCurrency
        };
    }

    public async Task SaveAsync(SaveSupplierPurchasingInfoDto dto)
    {
        await ApplyAsync(dto, false);
        await repository.SaveChangesAsync();
    }

    public async Task<int> SaveBulkAsync(BulkSaveSupplierPurchasingInfoDto dto)
    {
        if (await supplierRepository.GetByIdAsync(dto.SupplierId) is null)
            throw new ArgumentException("Supplier was not found.");
        var selected = dto.Rows.Where(x => x.Selected).ToList();
        if (selected.Count == 0)
            throw new ArgumentException("Select at least one product to save.");
        if (selected.GroupBy(x => new { x.ProductId, x.PurchaseMeasurementUnitId }).Any(x => x.Count() > 1))
            throw new ArgumentException("Each product and purchase unit can appear only once in the bulk save.");
        if (selected.Where(x => x.IsPreferred).GroupBy(x => x.ProductId).Any(x => x.Count() > 1))
            throw new ArgumentException("Only one purchasing row can be preferred for each product.");

        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            foreach (var row in selected)
            {
                await ApplyAsync(new SaveSupplierPurchasingInfoDto
                {
                    Id = row.Id,
                    SupplierId = dto.SupplierId,
                    ProductId = row.ProductId,
                    PurchaseMeasurementUnitId = row.PurchaseMeasurementUnitId,
                    SupplierProductCode = row.SupplierProductCode,
                    SupplierDescription = row.SupplierDescription,
                    MinimumOrderQuantity = row.MinimumOrderQuantity,
                    OrderMultiple = row.OrderMultiple,
                    LeadTimeDays = row.LeadTimeDays,
                    UnitPrice = row.UnitPrice,
                    CurrencyCode = row.CurrencyCode,
                    ValidFrom = row.ValidFrom,
                    ValidTo = row.ValidTo,
                    IsPreferred = row.IsPreferred,
                    Priority = row.Priority,
                    RowVersion = row.RowVersion
                }, true);
            }
        });
        return selected.Count;
    }

    private async Task ApplyAsync(SaveSupplierPurchasingInfoDto dto, bool preserveIdentity)
    {
        if (await supplierRepository.GetByIdAsync(dto.SupplierId) is null)
            throw new ArgumentException("Supplier was not found.");
        var product = await productRepository.GetByIdAsync(dto.ProductId)
            ?? throw new ArgumentException("Product was not found.");
        var purchaseUnit = await unitRepository.GetByIdAsync(dto.PurchaseMeasurementUnitId)
            ?? throw new ArgumentException("Purchase unit was not found.");
        var settings = await generalSettingsRepository.GetAsync();
        var baseCurrency = NormalizeBaseCurrency(settings?.Currency);

        if (product.ProductType == ProductType.PreparedToOrder)
            throw new InvalidOperationException("Prepared-to-order products are costed from recipes and cannot have supplier purchasing terms.");
        if (!product.IsActive)
            throw new InvalidOperationException("Supplier purchasing terms require an active product.");
        if (!purchaseUnit.IsActive)
            throw new InvalidOperationException("Supplier purchasing terms require an active purchase unit.");
        if (!product.MeasurementUnitId.HasValue)
            throw new InvalidOperationException("The product must have a managed stock unit before supplier purchasing terms are added.");
        var stockUnit = await unitRepository.GetByIdAsync(product.MeasurementUnitId.Value)
            ?? throw new InvalidOperationException("The product stock unit was not found.");
        if (stockUnit.Dimension != purchaseUnit.Dimension)
            throw new InvalidOperationException("Purchase unit and product stock unit must use the same measurement dimension.");
        if (!string.Equals(dto.CurrencyCode?.Trim(), baseCurrency, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("PUR-010 supports the company base currency only. Foreign-currency posting is not enabled.");

        var duplicate = await repository.FindDuplicateAsync(
            dto.SupplierId, dto.ProductId, dto.PurchaseMeasurementUnitId, dto.Id > 0 ? dto.Id : null);
        if (duplicate is not null)
            throw new InvalidOperationException("Purchasing information already exists for this supplier, product and purchase unit.");

        SupplierProductPurchasingInfo info;
        if (dto.Id == 0)
        {
            info = new SupplierProductPurchasingInfo(dto.SupplierId, dto.ProductId,
                dto.PurchaseMeasurementUnitId, dto.SupplierProductCode, dto.SupplierDescription,
                dto.MinimumOrderQuantity, dto.OrderMultiple, dto.LeadTimeDays, dto.UnitPrice,
                baseCurrency, dto.ValidFrom, dto.ValidTo, dto.IsPreferred, dto.Priority);
            await repository.AddAsync(info);
        }
        else
        {
            info = await repository.GetByIdAsync(dto.Id)
                ?? throw new InvalidOperationException("Supplier purchasing information was not found.");
            if (preserveIdentity && (info.SupplierId != dto.SupplierId || info.ProductId != dto.ProductId))
                throw new InvalidOperationException("Bulk purchasing rows must remain with their original supplier and product.");
            if (!info.RowVersion.SequenceEqual(dto.RowVersion))
                throw new InvalidOperationException("Supplier purchasing information was modified by another user. Reload and try again.");
            info.Update(dto.SupplierId, dto.ProductId, dto.PurchaseMeasurementUnitId,
                dto.SupplierProductCode, dto.SupplierDescription, dto.MinimumOrderQuantity,
                dto.OrderMultiple, dto.LeadTimeDays, dto.UnitPrice, baseCurrency,
                dto.ValidFrom, dto.ValidTo, dto.IsPreferred, dto.Priority);
        }

        if (dto.IsPreferred)
        {
            foreach (var current in await repository.GetPreferredForProductAsync(dto.ProductId, dto.Id > 0 ? dto.Id : null))
                current.SetPreferred(false);
        }

    }

    public async Task SetActiveAsync(int id, bool isActive, byte[] rowVersion)
    {
        var info = await repository.GetByIdAsync(id)
            ?? throw new InvalidOperationException("Supplier purchasing information was not found.");
        if (!info.RowVersion.SequenceEqual(rowVersion))
            throw new InvalidOperationException("Supplier purchasing information was modified by another user. Reload and try again.");
        info.SetActive(isActive);
        await repository.SaveChangesAsync();
    }

    private static string NormalizeBaseCurrency(string? currency) =>
        string.IsNullOrWhiteSpace(currency) ? "JOD" : currency.Trim().ToUpperInvariant();

    private static List<BulkSupplierPurchasingRowDto> BuildBulkRows(
        List<Product> products, List<MeasurementUnit> units,
        List<SupplierProductPurchasingInfo> infos, string baseCurrency)
    {
        var activeUnits = units.Where(x => x.IsActive).ToList();
        var unitsById = units.ToDictionary(x => x.Id);
        var rows = new List<BulkSupplierPurchasingRowDto>();
        foreach (var product in products.Where(x => x.IsActive && x.ProductType != ProductType.PreparedToOrder)
                     .OrderBy(x => x.Name))
        {
            if (!product.MeasurementUnitId.HasValue ||
                !unitsById.TryGetValue(product.MeasurementUnitId.Value, out var stockUnit)) continue;
            var options = activeUnits.Where(x => x.Dimension == stockUnit.Dimension)
                .OrderBy(x => x.Name)
                .Select(x => new SupplierPurchasingOptionDto(x.Id, $"{x.Name} ({x.Symbol})"))
                .ToList();
            var existing = infos.Where(x => x.ProductId == product.Id).OrderBy(x => x.Priority).ToList();
            if (existing.Count == 0)
            {
                rows.Add(new BulkSupplierPurchasingRowDto
                {
                    ProductId = product.Id, ProductCode = product.ProductCode, ProductName = product.Name,
                    PurchaseMeasurementUnitId = product.MeasurementUnitId.Value, UnitOptions = options,
                    SupplierProductCode = product.ProductCode, CurrencyCode = baseCurrency
                });
                continue;
            }
            rows.AddRange(existing.Select(info => new BulkSupplierPurchasingRowDto
            {
                Id = info.Id, ProductId = product.Id, ProductCode = product.ProductCode, ProductName = product.Name,
                PurchaseMeasurementUnitId = info.PurchaseMeasurementUnitId, UnitOptions = options,
                SupplierProductCode = info.SupplierProductCode, SupplierDescription = info.SupplierDescription,
                MinimumOrderQuantity = info.MinimumOrderQuantity, OrderMultiple = info.OrderMultiple,
                LeadTimeDays = info.LeadTimeDays, UnitPrice = info.UnitPrice, CurrencyCode = info.CurrencyCode,
                ValidFrom = info.ValidFrom, ValidTo = info.ValidTo, IsPreferred = info.IsPreferred,
                Priority = info.Priority, IsActive = info.IsActive, RowVersion = info.RowVersion
            }));
        }
        return rows;
    }
}
