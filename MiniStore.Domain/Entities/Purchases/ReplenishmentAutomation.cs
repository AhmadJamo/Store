namespace MiniStore.Domain.Entities;

public enum ReplenishmentAutomationMode { ReviewOnly = 1, PurchaseRequestDraft = 2, DirectRfqWithRequestFallback = 3 }
public enum ReplenishmentRunStatus { Running = 1, Completed = 2, Failed = 3 }
public enum ReplenishmentSuggestionOutcome { NoAction = 1, ReviewRequired = 2, PurchaseRequestCreated = 3, DirectRfqCreated = 4, ExistingDemand = 5, Skipped = 6 }
public enum ReplenishmentSuggestionReason { TargetAlreadyCovered = 1, ActiveDemandExists = 2, SupplierTermsMissing = 3, BelowMaximumTarget = 4, BelowForecastTarget = 5 }

public sealed class ReplenishmentAutomationSettings
{
    private ReplenishmentAutomationSettings() { }
    public int Id { get; private set; }
    public int SingletonKey { get; private set; } = 1;
    public bool IsEnabled { get; private set; }
    public int FrequencyHours { get; private set; } = 24;
    public int ForecastLookbackDays { get; private set; } = 90;
    public ReplenishmentAutomationMode Mode { get; private set; } = ReplenishmentAutomationMode.ReviewOnly;
    public int SupplierResponseDays { get; private set; } = 7;
    public DateTime? LastRunAtUtc { get; private set; }
    public DateTime? NextRunAtUtc { get; private set; }
    public byte[] RowVersion { get; private set; } = [];

    public ReplenishmentAutomationSettings(bool enabled, int frequencyHours, int forecastLookbackDays,
        ReplenishmentAutomationMode mode, int supplierResponseDays, DateTime nowUtc)
        => Configure(enabled, frequencyHours, forecastLookbackDays, mode, supplierResponseDays, nowUtc);

    public void Configure(bool enabled, int frequencyHours, int forecastLookbackDays,
        ReplenishmentAutomationMode mode, int supplierResponseDays, DateTime nowUtc)
    {
        if (frequencyHours is < 1 or > 168) throw new ArgumentException("Replenishment frequency must be between 1 and 168 hours.");
        if (forecastLookbackDays is < 7 or > 730) throw new ArgumentException("Forecast lookback must be between 7 and 730 days.");
        if (!Enum.IsDefined(mode)) throw new ArgumentException("Replenishment automation mode is invalid.");
        if (supplierResponseDays is < 1 or > 90) throw new ArgumentException("Supplier response days must be between 1 and 90.");
        IsEnabled = enabled; FrequencyHours = frequencyHours; ForecastLookbackDays = forecastLookbackDays;
        Mode = mode; SupplierResponseDays = supplierResponseDays;
        NextRunAtUtc = enabled ? nowUtc.AddHours(frequencyHours) : null;
    }

    public void MarkRun(DateTime runAtUtc)
    {
        LastRunAtUtc = runAtUtc;
        NextRunAtUtc = IsEnabled ? runAtUtc.AddHours(FrequencyHours) : null;
    }
}

public sealed class ReplenishmentRun
{
    private ReplenishmentRun() { TriggeredByUserId = string.Empty; }
    public long Id { get; private set; }
    public DateTime ScheduledForUtc { get; private set; }
    public DateTime StartedAtUtc { get; private set; }
    public DateTime? CompletedAtUtc { get; private set; }
    public ReplenishmentAutomationMode Mode { get; private set; }
    public ReplenishmentRunStatus Status { get; private set; }
    public string TriggeredByUserId { get; private set; }
    public bool IsScheduled { get; private set; }
    public string? Error { get; private set; }
    public List<ReplenishmentSuggestion> Suggestions { get; private set; } = [];

    public ReplenishmentRun(DateTime scheduledForUtc, ReplenishmentAutomationMode mode, string userId, bool scheduled)
    {
        if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("Replenishment run user is required.");
        ScheduledForUtc = scheduledForUtc; StartedAtUtc = DateTime.UtcNow; Mode = mode;
        TriggeredByUserId = userId.Trim(); IsScheduled = scheduled; Status = ReplenishmentRunStatus.Running;
    }
    public void AddSuggestion(ReplenishmentSuggestion suggestion) => Suggestions.Add(suggestion ?? throw new ArgumentNullException(nameof(suggestion)));
    public void Complete() { Status = ReplenishmentRunStatus.Completed; CompletedAtUtc = DateTime.UtcNow; }
    public void Fail(string error) { Status = ReplenishmentRunStatus.Failed; Error = error.Trim()[..Math.Min(error.Trim().Length, 1000)]; CompletedAtUtc = DateTime.UtcNow; }
}

public sealed class ReplenishmentSuggestion
{
    private ReplenishmentSuggestion() { ProductCodeSnapshot = ProductNameSnapshot = WarehouseNameSnapshot = PurchaseUnitSymbolSnapshot = CurrencyCode = string.Empty; }
    public long Id { get; private set; }
    public long ReplenishmentRunId { get; private set; }
    public int ReplenishmentRuleId { get; private set; }
    public int ProductId { get; private set; }
    public int WarehouseId { get; private set; }
    public int? SupplierId { get; private set; }
    public string ProductCodeSnapshot { get; private set; }
    public string ProductNameSnapshot { get; private set; }
    public string WarehouseNameSnapshot { get; private set; }
    public decimal AvailableQuantity { get; private set; }
    public decimal ReservedQuantity { get; private set; }
    public decimal IncomingQuantity { get; private set; }
    public decimal HistoricalDemandQuantity { get; private set; }
    public decimal SeasonalFactor { get; private set; }
    public decimal TargetQuantity { get; private set; }
    public decimal SuggestedStockQuantity { get; private set; }
    public decimal SuggestedPurchaseQuantity { get; private set; }
    public string PurchaseUnitSymbolSnapshot { get; private set; }
    public string CurrencyCode { get; private set; }
    public DateOnly NeededByDate { get; private set; }
    public ReplenishmentSuggestionOutcome Outcome { get; private set; }
    public int? PurchaseRequestId { get; private set; }
    public int? PurchaseSourcingEventId { get; private set; }
    public ReplenishmentSuggestionReason ReasonCode { get; private set; }

    public ReplenishmentSuggestion(int ruleId, int productId, int warehouseId, int? supplierId,
        string productCode, string productName, string warehouseName, decimal available, decimal reserved,
        decimal incoming, decimal historicalDemand, decimal seasonalFactor, decimal target, decimal suggestedStock,
        decimal suggestedPurchase, string purchaseUnitSymbol, string currencyCode, DateOnly neededByDate, ReplenishmentSuggestionOutcome outcome, ReplenishmentSuggestionReason reasonCode)
    {
        if (ruleId <= 0 || productId <= 0 || warehouseId <= 0) throw new ArgumentException("Replenishment suggestion references are required.");
        ReplenishmentRuleId = ruleId; ProductId = productId; WarehouseId = warehouseId; SupplierId = supplierId;
        ProductCodeSnapshot = productCode.Trim(); ProductNameSnapshot = productName.Trim(); WarehouseNameSnapshot = warehouseName.Trim();
        AvailableQuantity = available; ReservedQuantity = reserved; IncomingQuantity = incoming;
        HistoricalDemandQuantity = historicalDemand; SeasonalFactor = seasonalFactor; TargetQuantity = target;
        SuggestedStockQuantity = suggestedStock; SuggestedPurchaseQuantity = suggestedPurchase;
        if (!Enum.IsDefined(reasonCode)) throw new ArgumentException("Replenishment suggestion reason is invalid.");
        PurchaseUnitSymbolSnapshot = purchaseUnitSymbol.Trim(); CurrencyCode = currencyCode.Trim().ToUpperInvariant();
        NeededByDate = neededByDate; Outcome = outcome; ReasonCode = reasonCode;
    }
    public void LinkPurchaseRequest(int id) { if (id <= 0) throw new ArgumentException("Purchase request is required."); PurchaseRequestId = id; Outcome = ReplenishmentSuggestionOutcome.PurchaseRequestCreated; }
    public void LinkSourcingEvent(int id) { if (id <= 0) throw new ArgumentException("Purchase sourcing event is required."); PurchaseSourcingEventId = id; Outcome = ReplenishmentSuggestionOutcome.DirectRfqCreated; }
}
