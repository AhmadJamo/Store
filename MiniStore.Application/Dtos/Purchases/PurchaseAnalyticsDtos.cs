using MiniStore.Domain.Entities;

namespace MiniStore.Application.DTOs.Purchases;

public enum PurchaseAnalyticsGroupBy { Month = 1, Supplier = 2, Buyer = 3 }

public sealed class PurchaseAnalyticsQueryDto
{
    public DateOnly From { get; set; } = DateOnly.FromDateTime(DateTime.Today.AddDays(-89));
    public DateOnly To { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    public int? SupplierId { get; set; }
    public string? BuyerUserId { get; set; }
    public PurchaseOrderStatus? OrderStatus { get; set; }
    public PurchaseAnalyticsGroupBy GroupBy { get; set; } = PurchaseAnalyticsGroupBy.Month;
}

public sealed class PurchaseAnalyticsDto
{
    public PurchaseAnalyticsQueryDto Query { get; init; } = new();
    public List<PurchaseRequestOptionDto> SupplierOptions { get; init; } = [];
    public List<PurchaseAnalyticsBuyerOptionDto> BuyerOptions { get; set; } = [];
    public int OpenOrders { get; init; }
    public int UnconfirmedOrders { get; init; }
    public int OverdueOrders { get; init; }
    public decimal ReceiptValue { get; init; }
    public decimal VendorBillSpend { get; init; }
    public decimal LandedCostTotal { get; init; }
    public decimal OutstandingPayables { get; init; }
    public int MatchExceptionCount { get; init; }
    public int OpenMatchExceptionCount { get; init; }
    public int MatchOverrideCount { get; init; }
    public decimal AverageDeliveryDays { get; init; }
    public int LegacyPurchaseCount { get; init; }
    public decimal LegacyPurchaseValue { get; init; }
    public int ModernPurchaseOrderCount { get; init; }
    public decimal ModernPurchaseOrderValue { get; init; }
    public List<SupplierPerformanceDto> Suppliers { get; init; } = [];
    public List<PurchaseAnalyticsTrendDto> Trends { get; init; } = [];
    public List<PurchaseBuyerWorkloadDto> BuyerWorkloads { get; init; } = [];
    public List<PurchaseAnalyticsExceptionDto> OpenExceptions { get; init; } = [];
    public List<PurchaseAnalyticsKpiDto> Kpis { get; init; } = [];
    public List<PurchaseAcceptanceStageDto> AcceptanceJourney { get; init; } = [];
}

public sealed record PurchaseAnalyticsBuyerOptionDto(string UserId, string DisplayName);

public sealed class SupplierPerformanceDto
{
    public int SupplierId { get; init; }
    public string SupplierName { get; init; } = string.Empty;
    public int OrderCount { get; init; }
    public decimal OrderedValue { get; init; }
    public decimal ReceivedQuantity { get; init; }
    public decimal FillRatePercent { get; init; }
    public decimal OnTimeRatePercent { get; init; }
    public decimal AverageDeliveryDays { get; init; }
    public decimal ReturnRatePercent { get; init; }
    public decimal BilledSpend { get; init; }
    public decimal PriceVariance { get; init; }
    public decimal OutstandingBalance { get; init; }
    public int MatchExceptions { get; init; }
}

public sealed class PurchaseAnalyticsTrendDto
{
    public string GroupKey { get; init; } = string.Empty;
    public string GroupLabel { get; init; } = string.Empty;
    public int OrderCount { get; init; }
    public decimal OrderedValue { get; init; }
    public int BillCount { get; init; }
    public decimal Spend { get; init; }
    public decimal PriceVariance { get; init; }
    public decimal AverageUnitPrice { get; init; }
}

public sealed class PurchaseBuyerWorkloadDto
{
    public string BuyerUserId { get; init; } = string.Empty;
    public string BuyerDisplayName { get; set; } = string.Empty;
    public int OrderCount { get; init; }
    public int OpenOrders { get; init; }
    public int OverdueOrders { get; init; }
    public decimal OrderedValue { get; init; }
}

public sealed class PurchaseAnalyticsExceptionDto
{
    public int VendorBillId { get; init; }
    public string VendorBillNumber { get; init; } = string.Empty;
    public string SupplierName { get; init; } = string.Empty;
    public int ExceptionCount { get; init; }
    public decimal MaximumVariancePercent { get; init; }
}

public sealed record PurchaseAnalyticsKpiDto(string Label, string Value, string Source, string Period, string Explanation);
public sealed record PurchaseAcceptanceStageDto(string Label, int Count, string Controller, string Action, string RequiredPermission, string Explanation);
