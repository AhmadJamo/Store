namespace MiniStore.Application.DTOs.Purchases;

public sealed class PurchaseAnalyticsQueryDto
{
    public DateOnly From { get; set; } = DateOnly.FromDateTime(DateTime.Today.AddDays(-89));
    public DateOnly To { get; set; } = DateOnly.FromDateTime(DateTime.Today);
}

public sealed class PurchaseAnalyticsDto
{
    public PurchaseAnalyticsQueryDto Query { get; init; } = new();
    public int OpenOrders { get; init; }
    public int OverdueOrders { get; init; }
    public decimal ReceiptValue { get; init; }
    public decimal VendorBillSpend { get; init; }
    public decimal LandedCostTotal { get; init; }
    public decimal OutstandingPayables { get; init; }
    public int MatchExceptionCount { get; init; }
    public int MatchOverrideCount { get; init; }
    public List<SupplierPerformanceDto> Suppliers { get; init; } = [];
}

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
