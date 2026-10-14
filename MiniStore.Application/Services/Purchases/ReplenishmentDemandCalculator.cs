using MiniStore.Domain.Entities;

namespace MiniStore.Application.Services;

public static class ReplenishmentDemandCalculator
{
    public static decimal CalculateSeasonalFactor(decimal historicalDemand, int lookbackDays,
        decimal matchingSeasonDemand, int horizonDays)
    {
        if (lookbackDays <= 0 || horizonDays <= 0) throw new ArgumentException("Demand planning periods must be positive.");
        var dailyDemand = historicalDemand / lookbackDays;
        if (dailyDemand <= 0 || matchingSeasonDemand <= 0) return 1m;
        return Math.Clamp((matchingSeasonDemand / horizonDays) / dailyDemand, 0.5m, 2m);
    }

    public static decimal CalculateTarget(ReplenishmentPlanningMethod method, decimal minimum, decimal maximum,
        decimal safetyStock, int leadTimeDays, int coverageDays, decimal historicalDemand, int lookbackDays,
        decimal seasonalFactor)
    {
        if (!Enum.IsDefined(method)) throw new ArgumentException("Replenishment planning method is invalid.");
        if (method == ReplenishmentPlanningMethod.MinMax) return maximum;
        if (lookbackDays <= 0 || coverageDays <= 0 || leadTimeDays < 0)
            throw new ArgumentException("Demand planning periods are invalid.");
        var dailyDemand = historicalDemand / lookbackDays;
        return Math.Max(minimum, decimal.Round(dailyDemand * seasonalFactor * (coverageDays + leadTimeDays) + safetyStock, 6));
    }

    public static decimal CalculateSuggestedStock(decimal target, decimal available, decimal incoming) =>
        Math.Max(0, target - (available + incoming));

    public static bool IsActivePurchaseRequest(PurchaseRequestStatus status, bool hasActiveSourcing) =>
        status is PurchaseRequestStatus.Draft or PurchaseRequestStatus.Submitted or PurchaseRequestStatus.Approved ||
        status == PurchaseRequestStatus.Sourced && hasActiveSourcing;

    public static bool IsActiveSourcing(PurchaseSourcingStatus status, bool hasActiveOrder) =>
        status is PurchaseSourcingStatus.Draft or PurchaseSourcingStatus.Sent || hasActiveOrder;
}
