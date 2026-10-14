using MiniStore.Application.DTOs.Purchases;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using System.Globalization;

namespace MiniStore.Application.Services;

public sealed class SupplierPurchasingInfoService(
    ISupplierProductPurchasingInfoRepository repository,
    ISupplierRepository supplierRepository,
    IProductRepository productRepository,
    IMeasurementUnitRepository unitRepository,
    IGeneralSettingsRepository generalSettingsRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser)
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
        var today = DateOnly.FromDateTime(DateTime.Today);
        var priceExceptionCount = SupplierPriceExceptionAnalyzer.Analyze(infos, today).Count;

        var baseCurrency = NormalizeBaseCurrency(settings?.Currency);
        if (supplierId.HasValue && suppliers.All(x => x.Id != supplierId.Value))
            throw new InvalidOperationException("Supplier was not found.");
        var selectedInfos = supplierId.HasValue
            ? infos.Where(x => x.SupplierId == supplierId.Value && x.IsActive).ToList()
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
                PriceBreakCount = info.PriceBreaks.Count,
                RowVersion = info.RowVersion
            }).ToList(),
            Suppliers = suppliers.Select(x => new SupplierPurchasingOptionDto(x.Id, x.Name)).ToList(),
            Products = products.Where(x => selectedInfos.Any(info => info.ProductId == x.Id))
                .OrderBy(x => x.Name)
                .Select(x => new SupplierPurchasingOptionDto(x.Id, $"{x.ProductCode} — {x.Name}"))
                .ToList(),
            Units = units.Where(x => x.IsActive).OrderBy(x => x.Dimension).ThenBy(x => x.Name)
                .Select(x => new SupplierPurchasingOptionDto(x.Id, $"{x.Name} ({x.Symbol}) — {x.Dimension}"))
                .ToList(),
            Form = new SaveSupplierPurchasingInfoDto { SupplierId = supplierId ?? 0, CurrencyCode = baseCurrency },
            SelectedSupplierId = supplierId,
            BulkForm = new BulkSaveSupplierPurchasingInfoDto { SupplierId = supplierId ?? 0, Rows = bulkRows },
            BaseCurrency = baseCurrency,
            PriceExceptionCount = priceExceptionCount
        };
    }

    public async Task<SupplierPriceExceptionPageDto> GetPriceExceptionsPageAsync(
        SupplierPriceExceptionType? selectedType = null,
        int page = 1)
    {
        if (selectedType.HasValue && !Enum.IsDefined(selectedType.Value))
            throw new ArgumentException("Supplier price exception filter is invalid.");
        if (page < 1) throw new ArgumentException("Page number must be positive.");
        var infos = await repository.GetAllAsync();
        var suppliers = (await supplierRepository.GetAllAsync()).ToDictionary(x => x.Id);
        var products = (await productRepository.GetAllAsync(null)).ToDictionary(x => x.Id);
        var units = (await unitRepository.GetAllAsync()).ToDictionary(x => x.Id);
        var today = DateOnly.FromDateTime(DateTime.Today);
        var findings = SupplierPriceExceptionAnalyzer.Analyze(infos, today);
        var visible = selectedType.HasValue
            ? findings.Where(x => x.Type == selectedType.Value).ToList()
            : findings;

        var rows = visible.GroupBy(x => x.PurchasingInfo).Select(group =>
        {
            var info = group.Key;
            var product = products.GetValueOrDefault(info.ProductId);
            var unit = units.GetValueOrDefault(info.PurchaseMeasurementUnitId);
            return new SupplierPriceExceptionRowDto
            {
                PurchasingInfoId = info.Id,
                SupplierId = info.SupplierId,
                SupplierName = suppliers.GetValueOrDefault(info.SupplierId)?.Name ?? $"#{info.SupplierId}",
                ProductLabel = product is null ? $"#{info.ProductId}" : $"{product.ProductCode} — {product.Name}",
                PurchaseUnit = unit is null ? $"#{info.PurchaseMeasurementUnitId}" : $"{unit.Name} ({unit.Symbol})",
                SupplierProductCode = info.SupplierProductCode,
                UnitPrice = info.UnitPrice,
                CurrencyCode = info.CurrencyCode,
                ValidTo = info.ValidTo,
                IsPreferred = info.IsPreferred,
                Issues = group.Select(x => x.Type).Distinct().OrderBy(x => x).ToList()
            };
        }).OrderBy(x => x.Issues.Min()).ThenBy(x => x.SupplierName).ThenBy(x => x.ProductLabel).ToList();
        const int pageSize = 50;
        var totalPages = Math.Max(1, (int)Math.Ceiling(rows.Count / (double)pageSize));
        page = Math.Min(page, totalPages);

        return new SupplierPriceExceptionPageDto
        {
            AsOfDate = today,
            SelectedType = selectedType,
            TotalFindingCount = findings.Count,
            ExpiredCount = findings.Count(x => x.Type == SupplierPriceExceptionType.Expired),
            ExpiringSoonCount = findings.Count(x => x.Type == SupplierPriceExceptionType.ExpiringSoon),
            DuplicateSupplierProductCodeCount = findings.Count(x => x.Type == SupplierPriceExceptionType.DuplicateSupplierProductCode),
            MultiplePreferredRowsCount = findings.Count(x => x.Type == SupplierPriceExceptionType.MultiplePreferredRows),
            PageNumber = page,
            PageSize = pageSize,
            TotalRowCount = rows.Count,
            Rows = rows.Skip((page - 1) * pageSize).Take(pageSize).ToList()
        };
    }

    public async Task SaveAsync(SaveSupplierPurchasingInfoDto dto)
    {
        await ApplyAsync(dto, false, SupplierPriceChangeSource.ManualEdit);
        await repository.SaveChangesAsync();
    }

    public async Task<int> SaveBulkAsync(
        BulkSaveSupplierPurchasingInfoDto dto,
        SupplierPriceChangeSource source = SupplierPriceChangeSource.BulkEdit)
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
                }, true, source);
            }
        });
        return selected.Count;
    }

    public async Task<string> ExportCsvAsync(int supplierId)
    {
        if (await supplierRepository.GetByIdAsync(supplierId) is null)
            throw new ArgumentException("Supplier was not found.");
        var products = (await productRepository.GetAllAsync(null)).ToDictionary(x => x.Id);
        var units = (await unitRepository.GetAllAsync()).ToDictionary(x => x.Id);
        var rows = (await repository.GetBySupplierIdAsync(supplierId)).Where(x => x.IsActive)
            .OrderBy(x => products.GetValueOrDefault(x.ProductId)?.Name).ThenBy(x => x.Priority)
            .Select(info => (IReadOnlyList<string>)new[]
            {
                products.GetValueOrDefault(info.ProductId)?.ProductCode ?? string.Empty,
                units.GetValueOrDefault(info.PurchaseMeasurementUnitId)?.Code ?? string.Empty,
                info.SupplierProductCode, info.SupplierDescription ?? string.Empty,
                Invariant(info.MinimumOrderQuantity), Invariant(info.OrderMultiple),
                info.LeadTimeDays.ToString(CultureInfo.InvariantCulture), Invariant(info.UnitPrice),
                info.ValidFrom.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                info.ValidTo?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) ?? string.Empty,
                info.IsPreferred ? "TRUE" : "FALSE", info.Priority.ToString(CultureInfo.InvariantCulture)
            }).ToList();
        return SupplierPurchasingCsvParser.Write(rows);
    }

    public async Task<SupplierPurchasingCsvPreviewDto> PreviewCsvAsync(
        int supplierId, string csvContent, string fileName)
    {
        if (csvContent.Length > 1_048_576) throw new ArgumentException("The supplier purchasing CSV file cannot exceed 1 MB.");
        var supplier = await supplierRepository.GetByIdAsync(supplierId)
            ?? throw new ArgumentException("Supplier was not found.");
        var parsed = SupplierPurchasingCsvParser.Parse(csvContent);
        var products = await productRepository.GetAllAsync(null);
        var units = await unitRepository.GetAllAsync();
        var existing = await repository.GetBySupplierIdAsync(supplierId);
        var settings = await generalSettingsRepository.GetAsync();
        var baseCurrency = NormalizeBaseCurrency(settings?.Currency);
        var productMap = products.GroupBy(x => x.ProductCode, StringComparer.OrdinalIgnoreCase)
            .Where(x => x.Count() == 1).ToDictionary(x => x.Key, x => x.Single(), StringComparer.OrdinalIgnoreCase);
        var unitMap = units.GroupBy(x => x.Code, StringComparer.OrdinalIgnoreCase)
            .Where(x => x.Count() == 1).ToDictionary(x => x.Key, x => x.Single(), StringComparer.OrdinalIgnoreCase);
        var previewRows = new List<SupplierPurchasingCsvPreviewRowDto>();

        for (var index = 0; index < parsed.Count; index++)
        {
            var source = parsed[index];
            var errors = new List<string>();
            productMap.TryGetValue(source["ProductCode"], out var product);
            unitMap.TryGetValue(source["PurchaseUnitCode"], out var unit);
            if (product is null) errors.Add("Product code was not found or is not unique.");
            if (unit is null) errors.Add("Purchase unit code was not found or is not unique.");
            decimal? minimum = Decimal(source["MinimumOrderQuantity"], "Minimum order quantity is invalid.", errors);
            decimal? multiple = Decimal(source["OrderMultiple"], "Order multiple is invalid.", errors);
            int? leadTime = Integer(source["LeadTimeDays"], "Lead time is invalid.", errors);
            decimal? unitPrice = Decimal(source["UnitPrice"], "Supplier unit price is invalid.", errors);
            DateOnly? validFrom = Date(source["ValidFrom"], false, "Valid-from date must use yyyy-MM-dd format.", errors);
            DateOnly? validTo = Date(source["ValidTo"], true, "Valid-to date must use yyyy-MM-dd format.", errors);
            bool? preferred = Boolean(source["IsPreferred"], "Preferred value must be TRUE or FALSE.", errors);
            int? priority = Integer(source["Priority"], "Supplier priority is invalid.", errors);
            SupplierProductPurchasingInfo? current = null;
            if (product is not null && unit is not null)
            {
                if (!existing.Any(x => x.ProductId == product.Id && x.IsActive))
                    errors.Add("The product is not assigned to this supplier.");
                current = existing.SingleOrDefault(x => x.ProductId == product.Id && x.PurchaseMeasurementUnitId == unit.Id);
                if (current is { IsActive: false }) errors.Add("The matching supplier purchasing row is inactive. Activate it before importing.");
                if (!product.IsActive) errors.Add("Supplier purchasing terms require an active product.");
                if (product.ProductType == ProductType.PreparedToOrder) errors.Add("Prepared-to-order products are costed from recipes and cannot have supplier purchasing terms.");
                if (!unit.IsActive) errors.Add("Supplier purchasing terms require an active purchase unit.");
                if (!product.MeasurementUnitId.HasValue) errors.Add("The product must have a managed stock unit before supplier purchasing terms are added.");
                else if (units.SingleOrDefault(x => x.Id == product.MeasurementUnitId.Value) is not { } stockUnit)
                    errors.Add("The product stock unit was not found.");
                else if (stockUnit.Dimension != unit.Dimension) errors.Add("Purchase unit and product stock unit must use the same measurement dimension.");
            }

            SaveSupplierPurchasingInfoDto? input = null;
            if (errors.Count == 0 && product is not null && unit is not null && minimum.HasValue && multiple.HasValue &&
                leadTime.HasValue && unitPrice.HasValue && validFrom.HasValue && preferred.HasValue && priority.HasValue)
            {
                try
                {
                    _ = new SupplierProductPurchasingInfo(supplierId, product.Id, unit.Id, source["SupplierProductCode"],
                        source["SupplierDescription"], minimum.Value, multiple.Value, leadTime.Value, unitPrice.Value,
                        baseCurrency, validFrom.Value, validTo, preferred.Value, priority.Value);
                    input = new SaveSupplierPurchasingInfoDto
                    {
                        Id = current?.Id ?? 0, SupplierId = supplierId, ProductId = product.Id,
                        PurchaseMeasurementUnitId = unit.Id, SupplierProductCode = source["SupplierProductCode"],
                        SupplierDescription = source["SupplierDescription"], MinimumOrderQuantity = minimum.Value,
                        OrderMultiple = multiple.Value, LeadTimeDays = leadTime.Value, UnitPrice = unitPrice.Value,
                        CurrencyCode = baseCurrency, ValidFrom = validFrom.Value, ValidTo = validTo,
                        IsPreferred = preferred.Value, Priority = priority.Value, RowVersion = current?.RowVersion ?? []
                    };
                }
                catch (ArgumentException exception) { errors.Add(exception.Message); }
            }
            previewRows.Add(new SupplierPurchasingCsvPreviewRowDto
            {
                RowNumber = index + 2, ChangeType = current is null ? "Create" : "Update",
                ProductCode = source["ProductCode"], ProductName = product?.Name ?? string.Empty,
                PurchaseUnitCode = source["PurchaseUnitCode"], SupplierProductCode = source["SupplierProductCode"],
                UnitPrice = unitPrice, CurrencyCode = baseCurrency, Errors = errors, ValidatedInput = input
            });
        }

        foreach (var duplicate in previewRows.Where(x => x.ValidatedInput is not null)
                     .GroupBy(x => new { x.ValidatedInput!.ProductId, x.ValidatedInput.PurchaseMeasurementUnitId }).Where(x => x.Count() > 1))
            foreach (var row in duplicate) row.Errors.Add("The CSV file contains the same product and purchase unit more than once.");
        foreach (var duplicate in previewRows.Where(x => x.ValidatedInput?.IsPreferred == true)
                     .GroupBy(x => x.ValidatedInput!.ProductId).Where(x => x.Count() > 1))
            foreach (var row in duplicate) row.Errors.Add("Only one purchasing row can be preferred for each product.");

        return new SupplierPurchasingCsvPreviewDto
        {
            SupplierId = supplierId, SupplierName = supplier.Name, FileName = fileName.Trim(),
            CsvContent = csvContent, Rows = previewRows
        };
    }

    public async Task<int> ImportCsvAsync(ImportSupplierPurchasingCsvDto dto)
    {
        var preview = await PreviewCsvAsync(dto.SupplierId, dto.CsvContent, dto.FileName);
        if (!preview.CanImport) throw new InvalidOperationException("Resolve every CSV validation error before importing supplier purchasing data.");
        return await SaveBulkAsync(new BulkSaveSupplierPurchasingInfoDto
        {
            SupplierId = dto.SupplierId,
            Rows = preview.Rows.Select(row => row.ValidatedInput!).Select(input => new BulkSupplierPurchasingRowDto
            {
                Selected = true, Id = input.Id, ProductId = input.ProductId,
                PurchaseMeasurementUnitId = input.PurchaseMeasurementUnitId, SupplierProductCode = input.SupplierProductCode,
                SupplierDescription = input.SupplierDescription, MinimumOrderQuantity = input.MinimumOrderQuantity,
                OrderMultiple = input.OrderMultiple, LeadTimeDays = input.LeadTimeDays, UnitPrice = input.UnitPrice,
                CurrencyCode = input.CurrencyCode, ValidFrom = input.ValidFrom, ValidTo = input.ValidTo,
                IsPreferred = input.IsPreferred, Priority = input.Priority, RowVersion = input.RowVersion
            }).ToList()
        }, SupplierPriceChangeSource.CsvImport);
    }

    public async Task<SupplierPriceBreakPageDto> GetPriceBreaksPageAsync(int purchasingInfoId)
    {
        var info = await repository.GetByIdAsync(purchasingInfoId)
            ?? throw new InvalidOperationException("Supplier purchasing information was not found.");
        var supplier = await supplierRepository.GetByIdAsync(info.SupplierId)
            ?? throw new InvalidOperationException("Supplier was not found.");
        var product = await productRepository.GetByIdAsync(info.ProductId)
            ?? throw new InvalidOperationException("Product was not found.");
        var unit = await unitRepository.GetByIdAsync(info.PurchaseMeasurementUnitId)
            ?? throw new InvalidOperationException("Purchase unit was not found.");

        return new SupplierPriceBreakPageDto
        {
            PurchasingInfoId = info.Id,
            SupplierId = info.SupplierId,
            SupplierName = supplier.Name,
            ProductLabel = $"{product.ProductCode} — {product.Name}",
            PurchaseUnit = $"{unit.Name} ({unit.Symbol})",
            MinimumOrderQuantity = info.MinimumOrderQuantity,
            BaseUnitPrice = info.UnitPrice,
            CurrencyCode = info.CurrencyCode,
            IsActive = info.IsActive,
            Form = new SaveSupplierPriceBreaksDto
            {
                PurchasingInfoId = info.Id,
                SupplierId = info.SupplierId,
                RowVersion = info.RowVersion,
                Rows = info.PriceBreaks.OrderBy(x => x.MinimumQuantity).Select(x => new SaveSupplierPriceBreakRowDto
                {
                    Selected = true,
                    MinimumQuantity = x.MinimumQuantity,
                    UnitPrice = x.UnitPrice,
                    DiscountPercent = x.DiscountPercent
                }).ToList()
            }
        };
    }

    public async Task<int> SavePriceBreaksAsync(SaveSupplierPriceBreaksDto dto)
    {
        var info = await repository.GetByIdAsync(dto.PurchasingInfoId)
            ?? throw new InvalidOperationException("Supplier purchasing information was not found.");
        if (info.SupplierId != dto.SupplierId)
            throw new InvalidOperationException("The price-break supplier does not match the purchasing row.");
        if (!info.RowVersion.SequenceEqual(dto.RowVersion))
            throw new InvalidOperationException("Supplier purchasing information was modified by another user. Reload and try again.");

        var priceBreaks = dto.Rows.Where(x => x.Selected)
            .Select(x => new SupplierProductPriceBreak(x.MinimumQuantity, x.UnitPrice, x.DiscountPercent))
            .ToList();
        if (PriceBreaksMatch(info.PriceBreaks, priceBreaks)) return info.SupplierId;
        EnsureBaselineSnapshot(info);
        info.ReplacePriceBreaks(priceBreaks);
        info.RecordPriceSnapshot(SupplierPriceChangeSource.PriceBreakEdit, currentUser.UserId);
        await repository.SaveChangesAsync();
        return info.SupplierId;
    }

    public async Task<SupplierPriceHistoryPageDto> GetPriceHistoryPageAsync(int purchasingInfoId)
    {
        var info = await repository.GetByIdAsync(purchasingInfoId)
            ?? throw new InvalidOperationException("Supplier purchasing information was not found.");
        var supplier = await supplierRepository.GetByIdAsync(info.SupplierId)
            ?? throw new InvalidOperationException("Supplier was not found.");
        var product = await productRepository.GetByIdAsync(info.ProductId)
            ?? throw new InvalidOperationException("Product was not found.");
        var unit = await unitRepository.GetByIdAsync(info.PurchaseMeasurementUnitId)
            ?? throw new InvalidOperationException("Purchase unit was not found.");
        var history = await repository.GetPriceHistoryAsync(info.Id);

        return new SupplierPriceHistoryPageDto
        {
            PurchasingInfoId = info.Id,
            SupplierId = info.SupplierId,
            SupplierName = supplier.Name,
            ProductLabel = $"{product.ProductCode} — {product.Name}",
            PurchaseUnit = $"{unit.Name} ({unit.Symbol})",
            Rows = history.Select(x => new SupplierPriceHistoryRowDto
            {
                Revision = x.Revision,
                Source = x.Source,
                MinimumOrderQuantity = x.MinimumOrderQuantity,
                BaseUnitPrice = x.BaseUnitPrice,
                CurrencyCode = x.CurrencyCode,
                ValidFrom = x.ValidFrom,
                ValidTo = x.ValidTo,
                ChangedByUserId = x.ChangedByUserId,
                ChangedAtUtc = x.ChangedAtUtc,
                PriceBreaks = x.PriceBreaks.OrderBy(tier => tier.MinimumQuantity)
                    .Select(tier => new SupplierPriceHistoryBreakDto
                    {
                        MinimumQuantity = tier.MinimumQuantity,
                        UnitPrice = tier.UnitPrice,
                        DiscountPercent = tier.DiscountPercent,
                        NetUnitPrice = tier.NetUnitPrice
                    }).ToList()
            }).ToList()
        };
    }

    private async Task ApplyAsync(
        SaveSupplierPurchasingInfoDto dto,
        bool preserveIdentity,
        SupplierPriceChangeSource source)
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
            info.RecordPriceSnapshot(source, currentUser.UserId);
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
            var pricingChanged = info.SupplierId != dto.SupplierId || info.ProductId != dto.ProductId ||
                                 info.PurchaseMeasurementUnitId != dto.PurchaseMeasurementUnitId ||
                                 info.MinimumOrderQuantity != dto.MinimumOrderQuantity ||
                                 info.UnitPrice != dto.UnitPrice ||
                                 !string.Equals(info.CurrencyCode, baseCurrency, StringComparison.OrdinalIgnoreCase) ||
                                 info.ValidFrom != dto.ValidFrom || info.ValidTo != dto.ValidTo;
            if (pricingChanged) EnsureBaselineSnapshot(info);
            info.Update(dto.SupplierId, dto.ProductId, dto.PurchaseMeasurementUnitId,
                dto.SupplierProductCode, dto.SupplierDescription, dto.MinimumOrderQuantity,
                dto.OrderMultiple, dto.LeadTimeDays, dto.UnitPrice, baseCurrency,
                dto.ValidFrom, dto.ValidTo, dto.IsPreferred, dto.Priority);
            if (pricingChanged) info.RecordPriceSnapshot(source, currentUser.UserId);
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

    private static string Invariant(decimal value) => value.ToString("0.######", CultureInfo.InvariantCulture);
    private void EnsureBaselineSnapshot(SupplierProductPurchasingInfo info)
    {
        if (info.PriceHistoryRevision == 0)
            info.RecordPriceSnapshot(SupplierPriceChangeSource.LegacyBaseline, currentUser.UserId);
    }

    private static bool PriceBreaksMatch(
        IEnumerable<SupplierProductPriceBreak> current,
        IEnumerable<SupplierProductPriceBreak> replacement) =>
        current.OrderBy(x => x.MinimumQuantity).Select(x => new { x.MinimumQuantity, x.UnitPrice, x.DiscountPercent })
            .SequenceEqual(replacement.OrderBy(x => x.MinimumQuantity)
                .Select(x => new { x.MinimumQuantity, x.UnitPrice, x.DiscountPercent }));
    private static decimal? Decimal(string value, string error, List<string> errors)
    {
        if (decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out var parsed)) return parsed;
        errors.Add(error); return null;
    }
    private static int? Integer(string value, string error, List<string> errors)
    {
        if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed)) return parsed;
        errors.Add(error); return null;
    }
    private static DateOnly? Date(string value, bool optional, string error, List<string> errors)
    {
        if (optional && string.IsNullOrWhiteSpace(value)) return null;
        if (DateOnly.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed)) return parsed;
        errors.Add(error); return null;
    }
    private static bool? Boolean(string value, string error, List<string> errors)
    {
        if (bool.TryParse(value, out var parsed)) return parsed;
        if (value == "1") return true;
        if (value == "0") return false;
        errors.Add(error); return null;
    }

    private static List<BulkSupplierPurchasingRowDto> BuildBulkRows(
        List<Product> products, List<MeasurementUnit> units,
        List<SupplierProductPurchasingInfo> infos, string baseCurrency)
    {
        var activeUnits = units.Where(x => x.IsActive).ToList();
        var unitsById = units.ToDictionary(x => x.Id);
        var rows = new List<BulkSupplierPurchasingRowDto>();
        foreach (var product in products.Where(x => infos.Any(info => info.ProductId == x.Id))
                     .OrderBy(x => x.Name))
        {
            if (!product.MeasurementUnitId.HasValue ||
                !unitsById.TryGetValue(product.MeasurementUnitId.Value, out var stockUnit)) continue;
            var options = activeUnits.Where(x => x.Dimension == stockUnit.Dimension)
                .OrderBy(x => x.Name)
                .Select(x => new SupplierPurchasingOptionDto(x.Id, $"{x.Name} ({x.Symbol})"))
                .ToList();
            var existing = infos.Where(x => x.ProductId == product.Id).OrderBy(x => x.Priority).ToList();
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
