namespace MiniStore.Domain.Entities;

public enum SupplierPriceChangeSource
{
    Assigned = 1,
    LegacyBaseline = 2,
    ManualEdit = 3,
    BulkEdit = 4,
    CsvImport = 5,
    PriceBreakEdit = 6,
    ExcelImport = 7
}

public sealed class SupplierProductPurchasingInfo
{
    private SupplierProductPurchasingInfo()
    {
        SupplierProductCode = string.Empty;
        CurrencyCode = string.Empty;
    }

    public int Id { get; private set; }
    public int SupplierId { get; private set; }
    public int ProductId { get; private set; }
    public int PurchaseMeasurementUnitId { get; private set; }
    public string SupplierProductCode { get; private set; } = string.Empty;
    public string? SupplierDescription { get; private set; }
    public decimal MinimumOrderQuantity { get; private set; }
    public decimal OrderMultiple { get; private set; }
    public int LeadTimeDays { get; private set; }
    public decimal UnitPrice { get; private set; }
    public string CurrencyCode { get; private set; } = string.Empty;
    public DateOnly ValidFrom { get; private set; }
    public DateOnly? ValidTo { get; private set; }
    public bool IsPreferred { get; private set; }
    public int Priority { get; private set; }
    public bool IsActive { get; private set; }
    public int PriceBreakRevision { get; private set; }
    public int PriceHistoryRevision { get; private set; }
    public byte[] RowVersion { get; private set; } = [];
    public List<SupplierProductPriceBreak> PriceBreaks { get; private set; } = [];
    public List<SupplierProductPriceHistory> PriceHistory { get; private set; } = [];

    public SupplierProductPurchasingInfo(
        int supplierId,
        int productId,
        int purchaseMeasurementUnitId,
        string supplierProductCode,
        string? supplierDescription,
        decimal minimumOrderQuantity,
        decimal orderMultiple,
        int leadTimeDays,
        decimal unitPrice,
        string currencyCode,
        DateOnly validFrom,
        DateOnly? validTo,
        bool isPreferred,
        int priority)
    {
        Configure(supplierId, productId, purchaseMeasurementUnitId, supplierProductCode,
            supplierDescription, minimumOrderQuantity, orderMultiple, leadTimeDays,
            unitPrice, currencyCode, validFrom, validTo, isPreferred, priority);
        IsActive = true;
    }

    public void Update(
        int supplierId,
        int productId,
        int purchaseMeasurementUnitId,
        string supplierProductCode,
        string? supplierDescription,
        decimal minimumOrderQuantity,
        decimal orderMultiple,
        int leadTimeDays,
        decimal unitPrice,
        string currencyCode,
        DateOnly validFrom,
        DateOnly? validTo,
        bool isPreferred,
        int priority) =>
        Configure(supplierId, productId, purchaseMeasurementUnitId, supplierProductCode,
            supplierDescription, minimumOrderQuantity, orderMultiple, leadTimeDays,
            unitPrice, currencyCode, validFrom, validTo, isPreferred, priority);

    public void SetActive(bool isActive)
    {
        IsActive = isActive;
        if (!isActive) IsPreferred = false;
    }

    public void SetPreferred(bool isPreferred)
    {
        if (isPreferred && !IsActive)
            throw new InvalidOperationException("An inactive supplier purchasing record cannot be preferred.");
        IsPreferred = isPreferred;
    }

    public void ReplacePriceBreaks(IEnumerable<SupplierProductPriceBreak> priceBreaks)
    {
        if (!IsActive)
            throw new InvalidOperationException("Price breaks cannot be changed for inactive supplier purchasing information.");
        var replacement = priceBreaks.OrderBy(x => x.MinimumQuantity).ToList();
        if (replacement.Count > 20)
            throw new ArgumentException("A supplier purchasing row cannot contain more than 20 price breaks.");
        if (replacement.Any(x => x.MinimumQuantity < MinimumOrderQuantity))
            throw new ArgumentException("Each price break quantity must be at least the minimum order quantity.");
        if (replacement.GroupBy(x => x.MinimumQuantity).Any(x => x.Count() > 1))
            throw new ArgumentException("Each price break must use a unique minimum quantity.");

        PriceBreaks.Clear();
        PriceBreaks.AddRange(replacement);
        PriceBreakRevision++;
    }

    public decimal ResolveUnitPrice(decimal purchaseQuantity)
    {
        if (purchaseQuantity <= 0)
            throw new ArgumentException("Purchase quantity must be greater than zero.");
        return PriceBreaks
            .Where(x => purchaseQuantity >= x.MinimumQuantity)
            .OrderByDescending(x => x.MinimumQuantity)
            .Select(x => (decimal?)x.NetUnitPrice)
            .FirstOrDefault() ?? UnitPrice;
    }

    public void RecordPriceSnapshot(SupplierPriceChangeSource source, string userId)
    {
        if (!Enum.IsDefined(source))
            throw new ArgumentException("Supplier price change source is invalid.");
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("User is required.");

        PriceHistoryRevision++;
        PriceHistory.Add(new SupplierProductPriceHistory(
            PriceHistoryRevision, source, SupplierId, ProductId, PurchaseMeasurementUnitId,
            MinimumOrderQuantity, UnitPrice, CurrencyCode, ValidFrom, ValidTo,
            userId, PriceBreaks.Select(x => new SupplierProductPriceBreakSnapshot(
                x.MinimumQuantity, x.UnitPrice, x.DiscountPercent))));
    }

    private void Configure(
        int supplierId, int productId, int purchaseMeasurementUnitId,
        string supplierProductCode, string? supplierDescription,
        decimal minimumOrderQuantity, decimal orderMultiple, int leadTimeDays,
        decimal unitPrice, string currencyCode, DateOnly validFrom, DateOnly? validTo,
        bool isPreferred, int priority)
    {
        if (supplierId <= 0) throw new ArgumentException("Supplier is required.");
        if (productId <= 0) throw new ArgumentException("Product is required.");
        if (purchaseMeasurementUnitId <= 0) throw new ArgumentException("Purchase unit is required.");
        if (string.IsNullOrWhiteSpace(supplierProductCode) || supplierProductCode.Trim().Length > 100)
            throw new ArgumentException("Supplier product code is required and cannot exceed 100 characters.");
        if (supplierDescription?.Trim().Length > 300)
            throw new ArgumentException("Supplier description cannot exceed 300 characters.");
        if (minimumOrderQuantity <= 0)
            throw new ArgumentException("Minimum order quantity must be greater than zero.");
        if (PriceBreaks.Any(x => x.MinimumQuantity < minimumOrderQuantity))
            throw new ArgumentException("The minimum order quantity cannot exceed an existing price-break quantity.");
        if (orderMultiple <= 0)
            throw new ArgumentException("Order multiple must be greater than zero.");
        if (leadTimeDays is < 0 or > 3650)
            throw new ArgumentException("Lead time must be between 0 and 3650 days.");
        if (unitPrice < 0)
            throw new ArgumentException("Supplier unit price cannot be negative.");
        if (string.IsNullOrWhiteSpace(currencyCode) || currencyCode.Trim().Length != 3 ||
            !currencyCode.Trim().All(char.IsLetter))
            throw new ArgumentException("Currency code must contain exactly three letters.");
        if (validTo.HasValue && validTo.Value < validFrom)
            throw new ArgumentException("Valid-to date cannot be earlier than valid-from date.");
        if (priority is < 0 or > 9999)
            throw new ArgumentException("Supplier priority must be between 0 and 9999.");

        SupplierId = supplierId;
        ProductId = productId;
        PurchaseMeasurementUnitId = purchaseMeasurementUnitId;
        SupplierProductCode = supplierProductCode.Trim();
        SupplierDescription = string.IsNullOrWhiteSpace(supplierDescription) ? null : supplierDescription.Trim();
        MinimumOrderQuantity = minimumOrderQuantity;
        OrderMultiple = orderMultiple;
        LeadTimeDays = leadTimeDays;
        UnitPrice = unitPrice;
        CurrencyCode = currencyCode.Trim().ToUpperInvariant();
        ValidFrom = validFrom;
        ValidTo = validTo;
        IsPreferred = isPreferred;
        Priority = priority;
    }
}

public sealed class SupplierProductPriceBreak
{
    private SupplierProductPriceBreak() { }

    public int Id { get; private set; }
    public int SupplierProductPurchasingInfoId { get; private set; }
    public decimal MinimumQuantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal DiscountPercent { get; private set; }
    public decimal NetUnitPrice => decimal.Round(UnitPrice * (1m - DiscountPercent / 100m), 4, MidpointRounding.AwayFromZero);

    public SupplierProductPriceBreak(decimal minimumQuantity, decimal unitPrice, decimal discountPercent)
    {
        if (minimumQuantity <= 0)
            throw new ArgumentException("Price break minimum quantity must be greater than zero.");
        if (unitPrice < 0)
            throw new ArgumentException("Price break unit price cannot be negative.");
        if (discountPercent is < 0 or > 100)
            throw new ArgumentException("Price break discount must be between 0 and 100 percent.");

        MinimumQuantity = minimumQuantity;
        UnitPrice = unitPrice;
        DiscountPercent = discountPercent;
    }
}

public sealed class SupplierProductPriceHistory
{
    private SupplierProductPriceHistory()
    {
        CurrencyCode = ChangedByUserId = string.Empty;
    }

    public int Id { get; private set; }
    public int SupplierProductPurchasingInfoId { get; private set; }
    public int Revision { get; private set; }
    public SupplierPriceChangeSource Source { get; private set; }
    public int SupplierId { get; private set; }
    public int ProductId { get; private set; }
    public int PurchaseMeasurementUnitId { get; private set; }
    public decimal MinimumOrderQuantity { get; private set; }
    public decimal BaseUnitPrice { get; private set; }
    public string CurrencyCode { get; private set; } = string.Empty;
    public DateOnly ValidFrom { get; private set; }
    public DateOnly? ValidTo { get; private set; }
    public string ChangedByUserId { get; private set; } = string.Empty;
    public DateTime ChangedAtUtc { get; private set; }
    public List<SupplierProductPriceBreakSnapshot> PriceBreaks { get; private set; } = [];

    public SupplierProductPriceHistory(
        int revision, SupplierPriceChangeSource source, int supplierId, int productId,
        int purchaseMeasurementUnitId, decimal minimumOrderQuantity, decimal baseUnitPrice,
        string currencyCode, DateOnly validFrom, DateOnly? validTo, string changedByUserId,
        IEnumerable<SupplierProductPriceBreakSnapshot> priceBreaks)
    {
        if (revision <= 0) throw new ArgumentException("Supplier price history revision must be positive.");
        if (!Enum.IsDefined(source)) throw new ArgumentException("Supplier price change source is invalid.");
        if (supplierId <= 0 || productId <= 0 || purchaseMeasurementUnitId <= 0)
            throw new ArgumentException("Supplier price history references are required.");
        if (minimumOrderQuantity <= 0 || baseUnitPrice < 0)
            throw new ArgumentException("Supplier price history quantity and price are invalid.");
        if (string.IsNullOrWhiteSpace(currencyCode) || currencyCode.Trim().Length != 3)
            throw new ArgumentException("Currency code must contain exactly three letters.");
        if (validTo.HasValue && validTo.Value < validFrom)
            throw new ArgumentException("Valid-to date cannot be earlier than valid-from date.");
        if (string.IsNullOrWhiteSpace(changedByUserId)) throw new ArgumentException("User is required.");

        Revision = revision;
        Source = source;
        SupplierId = supplierId;
        ProductId = productId;
        PurchaseMeasurementUnitId = purchaseMeasurementUnitId;
        MinimumOrderQuantity = minimumOrderQuantity;
        BaseUnitPrice = baseUnitPrice;
        CurrencyCode = currencyCode.Trim().ToUpperInvariant();
        ValidFrom = validFrom;
        ValidTo = validTo;
        ChangedByUserId = changedByUserId.Trim();
        ChangedAtUtc = DateTime.UtcNow;
        PriceBreaks.AddRange(priceBreaks);
    }
}

public sealed class SupplierProductPriceBreakSnapshot
{
    private SupplierProductPriceBreakSnapshot() { }

    public int Id { get; private set; }
    public int SupplierProductPriceHistoryId { get; private set; }
    public decimal MinimumQuantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal DiscountPercent { get; private set; }
    public decimal NetUnitPrice => decimal.Round(UnitPrice * (1m - DiscountPercent / 100m), 4, MidpointRounding.AwayFromZero);

    public SupplierProductPriceBreakSnapshot(decimal minimumQuantity, decimal unitPrice, decimal discountPercent)
    {
        if (minimumQuantity <= 0 || unitPrice < 0 || discountPercent is < 0 or > 100)
            throw new ArgumentException("Supplier price-break history values are invalid.");
        MinimumQuantity = minimumQuantity;
        UnitPrice = unitPrice;
        DiscountPercent = discountPercent;
    }
}
