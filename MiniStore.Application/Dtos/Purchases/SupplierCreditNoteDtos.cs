using System.ComponentModel.DataAnnotations;
using MiniStore.Domain.Entities;

namespace MiniStore.Application.DTOs.Purchases;

public sealed class SupplierCreditNoteCreatePageDto
{
    public int VendorBillId { get; init; }
    public string VendorBillNumber { get; init; } = string.Empty;
    public string SupplierInvoiceNumber { get; init; } = string.Empty;
    public string SupplierName { get; init; } = string.Empty;
    public string CurrencyCode { get; init; } = string.Empty;
    public CreateSupplierCreditNoteDto Form { get; init; } = new();
    public List<SupplierCreditNoteAvailableLineDto> Lines { get; init; } = [];
}

public sealed class SupplierCreditNoteAvailableLineDto
{
    public int VendorBillLineId { get; init; }
    public string ProductCode { get; init; } = string.Empty;
    public string ProductName { get; init; } = string.Empty;
    public string UnitName { get; init; } = string.Empty;
    public decimal BilledQuantity { get; init; }
    public decimal CreditedQuantity { get; init; }
    public decimal RemainingQuantity { get; init; }
    public string? ReceivedTracking { get; init; }
}

public sealed class CreateSupplierCreditNoteDto
{
    [Range(1, int.MaxValue)]
    public int VendorBillId { get; set; }
    public DateOnly CreditDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    [StringLength(100)]
    public string? SupplierReference { get; set; }
    [Required, StringLength(1000)]
    public string Reason { get; set; } = string.Empty;
    [MinLength(1)]
    public List<CreateSupplierCreditNoteLineDto> Lines { get; set; } = [];
}

public sealed class CreateSupplierCreditNoteLineDto
{
    [Range(1, int.MaxValue)]
    public int VendorBillLineId { get; set; }
    [Range(typeof(decimal), "0", "999999999999", ParseLimitsInInvariantCulture = true, ConvertValueInInvariantCulture = true)]
    public decimal Quantity { get; set; }
    [StringLength(2000)]
    public string? TrackingAllocations { get; set; }
}

public sealed class SupplierCreditNoteDto
{
    public int Id { get; init; }
    public int VendorBillId { get; init; }
    public string CreditNoteNumber { get; init; } = string.Empty;
    public string VendorBillNumber { get; init; } = string.Empty;
    public string SupplierInvoiceNumber { get; init; } = string.Empty;
    public string SupplierName { get; init; } = string.Empty;
    public string WarehouseName { get; init; } = string.Empty;
    public DateOnly CreditDate { get; init; }
    public string CurrencyCode { get; init; } = string.Empty;
    public string? SupplierReference { get; init; }
    public string Reason { get; init; } = string.Empty;
    public SupplierCreditNoteStatus Status { get; init; }
    public decimal NetAmount { get; init; }
    public decimal TaxAmount { get; init; }
    public decimal TotalAmount { get; init; }
    public decimal RemovedInventoryCost { get; init; }
    public List<SupplierCreditNoteLineDto> Lines { get; init; } = [];
}

public sealed class SupplierCreditNoteLineDto
{
    public string ProductCode { get; init; } = string.Empty;
    public string ProductName { get; init; } = string.Empty;
    public string UnitName { get; init; } = string.Empty;
    public decimal Quantity { get; init; }
    public decimal StockQuantity { get; init; }
    public decimal NetAmount { get; init; }
    public decimal TaxAmount { get; init; }
    public decimal GrossAmount { get; init; }
    public decimal RemovedInventoryCost { get; init; }
    public string? TrackingAllocations { get; init; }
}
