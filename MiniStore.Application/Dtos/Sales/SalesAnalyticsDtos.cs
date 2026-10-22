using MiniStore.Domain.Entities;

namespace MiniStore.Application.Dtos.Sales;

public enum SalesAnalyticsGroupBy { Month = 1, Customer = 2, Product = 3 }

public sealed class SalesAnalyticsQueryDto
{
    public DateOnly From { get; set; } = DateOnly.FromDateTime(DateTime.Today.AddDays(-89));
    public DateOnly To { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    public int? CustomerId { get; set; }
    public SalesAnalyticsGroupBy GroupBy { get; set; } = SalesAnalyticsGroupBy.Month;
}

public sealed class SalesAnalyticsDto
{
    public SalesAnalyticsQueryDto Query { get; init; } = new();
    public List<SalesAnalyticsOptionDto> CustomerOptions { get; init; } = [];
    public int QuotationCount { get; init; }
    public decimal QuotationValue { get; init; }
    public decimal QuotationAcceptanceRate { get; init; }
    public int SalesOrderCount { get; init; }
    public decimal SalesOrderValue { get; init; }
    public decimal PostedInvoiceRevenue { get; init; }
    public decimal CollectedAmount { get; init; }
    public decimal OutstandingReceivables { get; init; }
    public int OverdueInvoiceCount { get; init; }
    public decimal CreditNoteValue { get; init; }
    public decimal DeliveryCost { get; init; }
    public decimal GrossContribution { get; init; }
    public decimal GrossContributionPercent { get; init; }
    public int LegacySaleCount { get; init; }
    public decimal LegacySaleValue { get; init; }
    public int ModernInvoiceCount { get; init; }
    public decimal ModernInvoiceValue { get; init; }
    public List<SalesAnalyticsFunnelStageDto> Funnel { get; init; } = [];
    public List<SalesAnalyticsTrendDto> Trends { get; init; } = [];
    public List<SalesCustomerPerformanceDto> Customers { get; init; } = [];
    public List<SalesProductPerformanceDto> Products { get; init; } = [];
    public List<SalesAnalyticsKpiDto> Kpis { get; init; } = [];
}

public sealed record SalesAnalyticsOptionDto(int Id, string Label);
public sealed record SalesAnalyticsFunnelStageDto(string Label, int Count, decimal RateFromPrevious,
    string Controller, string RequiredPermission, string Explanation);
public sealed record SalesAnalyticsTrendDto(string GroupKey, string GroupLabel, int InvoiceCount,
    decimal NetRevenue, decimal Tax, decimal GrossRevenue, decimal Credits, decimal Collected);
public sealed record SalesCustomerPerformanceDto(int CustomerId, string CustomerCode, string CustomerName,
    int QuotationCount, int OrderCount, int InvoiceCount, decimal Revenue, decimal Collected,
    decimal Outstanding, int OverdueInvoices);
public sealed record SalesProductPerformanceDto(int ProductId, string ProductCode, string ProductName,
    decimal InvoicedStockQuantity, decimal NetRevenue, decimal DeliveryCost,
    decimal GrossContribution, decimal GrossContributionPercent);
public sealed record SalesAnalyticsKpiDto(string Label, string Value, string Source, string Period, string Explanation);
