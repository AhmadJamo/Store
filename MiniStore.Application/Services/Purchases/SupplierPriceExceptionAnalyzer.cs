using MiniStore.Domain.Entities;
using MiniStore.Application.DTOs.Purchases;

namespace MiniStore.Application.Services;

public sealed record SupplierPriceExceptionFinding(
    SupplierProductPurchasingInfo PurchasingInfo,
    SupplierPriceExceptionType Type);

public static class SupplierPriceExceptionAnalyzer
{
    public static List<SupplierPriceExceptionFinding> Analyze(
        IEnumerable<SupplierProductPurchasingInfo> purchasingInfos,
        DateOnly asOfDate,
        int expiringWithinDays = 30)
    {
        if (expiringWithinDays is < 1 or > 365)
            throw new ArgumentException("Supplier price expiry horizon must be between 1 and 365 days.");

        var active = purchasingInfos.Where(x => x.IsActive).ToList();
        var findings = new List<SupplierPriceExceptionFinding>();
        var expiringThrough = asOfDate.AddDays(expiringWithinDays);

        foreach (var info in active.Where(x => x.ValidTo.HasValue))
        {
            if (info.ValidTo!.Value < asOfDate)
                findings.Add(new(info, SupplierPriceExceptionType.Expired));
            else if (info.ValidTo.Value <= expiringThrough)
                findings.Add(new(info, SupplierPriceExceptionType.ExpiringSoon));
        }

        foreach (var group in active.GroupBy(
                     x => $"{x.SupplierId}\u001f{x.SupplierProductCode}",
                     StringComparer.OrdinalIgnoreCase).Where(x => x.Count() > 1))
            findings.AddRange(group.Select(info =>
                new SupplierPriceExceptionFinding(info, SupplierPriceExceptionType.DuplicateSupplierProductCode)));

        foreach (var group in active.Where(x => x.IsPreferred).GroupBy(x => x.ProductId).Where(x => x.Count() > 1))
            findings.AddRange(group.Select(info =>
                new SupplierPriceExceptionFinding(info, SupplierPriceExceptionType.MultiplePreferredRows)));

        return findings;
    }
}
