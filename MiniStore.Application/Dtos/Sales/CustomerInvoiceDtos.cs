using System.ComponentModel.DataAnnotations;
using MiniStore.Domain.Entities;

namespace MiniStore.Application.Dtos.Sales;

public sealed record CustomerInvoiceSummaryDto(int Id, string InvoiceNumber, int SalesOrderId,
    string OrderNumber, string CustomerCode, string CustomerName, DateOnly InvoiceDate, DateOnly DueDate,
    string CurrencyCode, decimal TotalAmount, CustomerInvoiceStatus Status);

public sealed record CustomerInvoiceSourceDto(int SalesDeliveryId, int SalesDeliveryLineId,
    string DeliveryNumber, decimal StockQuantity);

public sealed class CustomerInvoiceLineDto
{
    public int Id { get; init; } public int SalesOrderLineId { get; init; }
    public string ProductCode { get; init; } = string.Empty; public string ProductName { get; init; } = string.Empty;
    public string UnitName { get; init; } = string.Empty; public decimal Quantity { get; init; }
    public decimal StockQuantity { get; init; } public SalesInvoiceQuantityBasis QuantityBasis { get; init; }
    public decimal UnitPrice { get; init; } public decimal NetAmount { get; init; }
    public string? TaxName { get; init; } public decimal? TaxRatePercent { get; init; }
    public decimal TaxAmount { get; init; } public decimal TotalAmount { get; init; }
    public List<CustomerInvoiceSourceDto> Sources { get; init; } = [];
}

public sealed record CustomerInvoiceHistoryDto(CustomerInvoiceStatus FromStatus, CustomerInvoiceStatus ToStatus,
    CustomerInvoiceHistoryAction Action, string UserId, string? Reason, DateTime CreatedAtUtc);

public sealed class CustomerInvoiceDetailsDto
{
    public int Id { get; init; } public string InvoiceNumber { get; init; } = string.Empty;
    public int SalesOrderId { get; init; } public string OrderNumber { get; init; } = string.Empty;
    public SalesInvoiceQuantityPolicy QuantityPolicy { get; init; }
    public string CustomerCode { get; init; } = string.Empty; public string CustomerName { get; init; } = string.Empty;
    public string? CustomerLegalName { get; init; } public string? CustomerTaxNumber { get; init; }
    public string? Address { get; init; } public DateOnly InvoiceDate { get; init; } public DateOnly DueDate { get; init; }
    public string CurrencyCode { get; init; } = string.Empty; public decimal ExchangeRate { get; init; }
    public DateOnly ExchangeRateDate { get; init; } public string ExchangeRateSource { get; init; } = string.Empty;
    public int PaymentTermDays { get; init; } public string? Notes { get; init; }
    public CustomerInvoiceStatus Status { get; init; } public decimal NetAmount { get; init; }
    public decimal TaxAmount { get; init; } public decimal TotalAmount { get; init; }
    public string CreatedByUserId { get; init; } = string.Empty; public DateTime CreatedAtUtc { get; init; }
    public string? ApprovedByUserId { get; init; } public DateTime? ApprovedAtUtc { get; init; }
    public string? CancellationReason { get; init; } public byte[] RowVersion { get; init; } = [];
    public List<CustomerInvoiceLineDto> Lines { get; init; } = [];
    public List<CustomerInvoiceHistoryDto> History { get; init; } = [];
}

public sealed class CustomerInvoiceEligibleLineDto
{
    public int SalesOrderLineId { get; init; } public string ProductCode { get; init; } = string.Empty;
    public string ProductName { get; init; } = string.Empty; public string UnitName { get; init; } = string.Empty;
    public SalesInvoiceQuantityBasis QuantityBasis { get; init; } public decimal OrderedQuantity { get; init; }
    public decimal SourceEligibleQuantity { get; init; } public decimal CommittedQuantity { get; init; }
    public decimal RemainingQuantity { get; init; } public decimal UnitPrice { get; init; }
}

public sealed class CustomerInvoiceCreatePageDto
{
    public int SalesOrderId { get; init; } public string OrderNumber { get; init; } = string.Empty;
    public string CustomerCode { get; init; } = string.Empty; public string CustomerName { get; init; } = string.Empty;
    public string CurrencyCode { get; init; } = string.Empty;
    public SalesInvoiceQuantityPolicy QuantityPolicy { get; init; }
    public List<CustomerInvoiceEligibleLineDto> Lines { get; init; } = [];
    public CreateCustomerInvoiceDto Form { get; init; } = new();
}

public sealed class CreateCustomerInvoiceLineDto
{
    [Range(1, int.MaxValue)] public int SalesOrderLineId { get; set; }
    [Range(typeof(decimal), "0", "999999999999.999999")] public decimal Quantity { get; set; }
}

public sealed class CreateCustomerInvoiceDto
{
    [Range(1, int.MaxValue)] public int SalesOrderId { get; set; }
    [Required, StringLength(100)] public string RequestKey { get; set; } = Guid.NewGuid().ToString("N");
    public DateOnly InvoiceDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    [StringLength(1000)] public string? Notes { get; set; }
    public List<CreateCustomerInvoiceLineDto> Lines { get; set; } = [];
}

public sealed class CustomerInvoiceStatusCommandDto
{
    [Range(1, int.MaxValue)] public int Id { get; set; }
    public byte[] RowVersion { get; set; } = [];
    [StringLength(500)] public string? Reason { get; set; }
}

public sealed class CustomerInvoiceProgressDto
{
    public int TotalOrderLineCount { get; init; }
    public int FullyInvoicedLineCount { get; init; }
    public int DraftInvoiceCount { get; init; }
    public int ApprovedInvoiceCount { get; init; }
    public bool HasEligibleRemaining { get; init; }
    public bool IsFullyInvoiced { get; init; }
    public decimal CommittedAmount { get; init; }
}
