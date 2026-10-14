using MiniStore.Domain.Entities;

namespace MiniStore.Application.DTOs.Purchases;

public sealed class SaveReplenishmentAutomationSettingsDto
{
    public bool IsEnabled { get; set; }
    public int FrequencyHours { get; set; } = 24;
    public int ForecastLookbackDays { get; set; } = 90;
    public ReplenishmentAutomationMode Mode { get; set; } = ReplenishmentAutomationMode.ReviewOnly;
    public int SupplierResponseDays { get; set; } = 7;
    public byte[] RowVersion { get; set; } = [];
}

public sealed class ReplenishmentAutomationPageDto
{
    public SaveReplenishmentAutomationSettingsDto Settings { get; init; } = new();
    public DateTime? LastRunAtUtc { get; init; }
    public DateTime? NextRunAtUtc { get; init; }
    public List<ReplenishmentRunDto> Runs { get; init; } = [];
}

public sealed class ReplenishmentRunDto
{
    public long Id { get; init; }
    public DateTime StartedAtUtc { get; init; }
    public DateTime? CompletedAtUtc { get; init; }
    public ReplenishmentAutomationMode Mode { get; init; }
    public ReplenishmentRunStatus Status { get; init; }
    public bool IsScheduled { get; init; }
    public string TriggeredByUserId { get; init; } = string.Empty;
    public string? Error { get; init; }
    public List<ReplenishmentSuggestionDto> Suggestions { get; init; } = [];
}

public sealed class ReplenishmentSuggestionDto
{
    public long Id { get; init; }
    public int RuleId { get; init; }
    public string Product { get; init; } = string.Empty;
    public string Warehouse { get; init; } = string.Empty;
    public decimal Available { get; init; }
    public decimal Reserved { get; init; }
    public decimal Incoming { get; init; }
    public decimal HistoricalDemand { get; init; }
    public decimal SeasonalFactor { get; init; }
    public decimal Target { get; init; }
    public decimal SuggestedStock { get; init; }
    public decimal SuggestedPurchase { get; init; }
    public string PurchaseUnitSymbol { get; init; } = string.Empty;
    public string CurrencyCode { get; init; } = string.Empty;
    public DateOnly NeededByDate { get; init; }
    public ReplenishmentSuggestionOutcome Outcome { get; init; }
    public int? PurchaseRequestId { get; init; }
    public int? PurchaseSourcingEventId { get; init; }
    public ReplenishmentSuggestionReason ReasonCode { get; init; }
}
