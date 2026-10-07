using MiniStore.Application.DTOs.Inventory.Insights;

namespace MiniStore.Application.Services;

public static class InventoryActivityClassifier
{
    public static InventoryActivityState Classify(
        DateTime? lastOutboundAt,
        DateTime? firstInboundAt,
        DateTime asOfUtc,
        int slowDays,
        int deadDays)
    {
        ValidateThresholds(slowDays, deadDays);

        var anchor = lastOutboundAt ?? firstInboundAt;
        if (!anchor.HasValue)
            return InventoryActivityState.NoMovementHistory;
        var ageDays = Math.Max(0, (asOfUtc.Date - anchor.Value.Date).Days);

        if (!lastOutboundAt.HasValue)
            return ageDays >= slowDays ? InventoryActivityState.NeverIssued : InventoryActivityState.Active;

        if (ageDays >= deadDays)
            return InventoryActivityState.Dead;

        return ageDays >= slowDays
            ? InventoryActivityState.Slow
            : InventoryActivityState.Active;
    }

    public static void ValidateThresholds(int slowDays, int deadDays)
    {
        if (slowDays < 1 || slowDays > 3650)
            throw new ArgumentException("Slow-stock days must be between 1 and 3650.");

        if (deadDays <= slowDays || deadDays > 3650)
            throw new ArgumentException("Dead-stock days must be greater than slow-stock days and no more than 3650.");
    }
}
