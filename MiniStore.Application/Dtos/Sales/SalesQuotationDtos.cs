using System.ComponentModel.DataAnnotations;
using MiniStore.Domain.Entities;

namespace MiniStore.Application.Dtos.Sales;

public sealed record SalesQuotationOptionDto(int Id, string Label);
public sealed record SalesQuotationAddressOptionDto(int Id, int CustomerId, string Label);
public sealed record SalesQuotationProductOptionDto(int Id, string Label, string SearchText);

public sealed class SalesQuotationCreatePageDto
{
    public CreateSalesQuotationDto Form { get; init; } = new();
    public List<SalesQuotationOptionDto> Customers { get; init; } = [];
    public List<SalesQuotationAddressOptionDto> Addresses { get; init; } = [];
    public List<SalesQuotationProductOptionDto> Products { get; init; } = [];
    public List<SalesQuotationOptionDto> Units { get; init; } = [];
    public List<SalesQuotationOptionDto> PriceLists { get; init; } = [];
    public List<SalesQuotationOptionDto> TaxRates { get; init; } = [];
}

public sealed class CreateSalesQuotationDto
{
    [Required, StringLength(36, MinimumLength = 36)] public string RequestKey { get; set; } = Guid.NewGuid().ToString();
    [Range(1, int.MaxValue)] public int CustomerId { get; set; }
    public int? CustomerAddressId { get; set; }
    public int? PriceListId { get; set; }
    public DateOnly QuotationDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    public DateOnly ValidUntil { get; set; } = DateOnly.FromDateTime(DateTime.Today.AddDays(30));
    public SaleChannel Channel { get; set; } = SaleChannel.Wholesale;
    [StringLength(1000)] public string? Notes { get; set; }
    public List<CreateSalesQuotationLineDto> Lines { get; set; } = [];
}

public sealed class CreateSalesQuotationLineDto
{
    [Range(1, int.MaxValue)] public int ProductId { get; set; }
    [Range(1, int.MaxValue)] public int MeasurementUnitId { get; set; }
    [Range(typeof(decimal), "0.000001", "999999999999")] public decimal Quantity { get; set; } = 1m;
    public int? TaxRateId { get; set; }
    [StringLength(500)] public string? Notes { get; set; }
}

public sealed record SalesQuotationSummaryDto(int Id, string QuotationNumber, string CustomerCode, string CustomerName,
    DateOnly QuotationDate, DateOnly ValidUntil, string CurrencyCode, decimal TotalAmount, SalesQuotationStatus Status);

public sealed class SalesQuotationDetailsDto
{
    public int Id { get; init; } public string QuotationNumber { get; init; } = ""; public SalesQuotationStatus Status { get; init; }
    public string CustomerCode { get; init; } = ""; public string CustomerName { get; init; } = ""; public string? CustomerLegalName { get; init; } public string? CustomerTaxNumber { get; init; }
    public string? Address { get; init; } public DateOnly QuotationDate { get; init; } public DateOnly ValidUntil { get; init; }
    public string CurrencyCode { get; init; } = ""; public decimal ExchangeRate { get; init; } public DateOnly ExchangeRateDate { get; init; } public string ExchangeRateSource { get; init; } = "";
    public int PaymentTermDays { get; init; } public string? Notes { get; init; } public decimal NetAmount { get; init; } public decimal TaxAmount { get; init; } public decimal TotalAmount { get; init; }
    public string CreatedByUserId { get; init; } = ""; public DateTime CreatedAtUtc { get; init; } public byte[] RowVersion { get; init; } = [];
    public int? SalesOrderId { get; init; }
    public List<SalesQuotationLineDto> Lines { get; init; } = []; public List<SalesQuotationHistoryDto> History { get; init; } = [];
}

public sealed record SalesQuotationLineDto(int Id, string ProductCode, string ProductName, string UnitCode, string UnitName,
    decimal Quantity, decimal StockQuantity, decimal UnitPrice, decimal CompanyUnitPrice, decimal NetAmount,
    string PriceSourceCode, string? TaxName, decimal? TaxRatePercent, bool IsTaxInclusive, string TaxSourceCode,
    decimal TaxAmount, decimal TotalAmount, string? Notes);
public sealed record SalesQuotationHistoryDto(SalesQuotationStatus FromStatus, SalesQuotationStatus ToStatus,
    SalesQuotationHistoryAction Action, string UserId, string? Reason, DateTime CreatedAtUtc);
public sealed class SalesQuotationStatusCommandDto
{
    [Range(1, int.MaxValue)] public int Id { get; set; }
    public byte[] RowVersion { get; set; } = [];
    [StringLength(500)] public string? Reason { get; set; }
}
