using System.ComponentModel.DataAnnotations;
using MiniStore.Domain.Entities;

namespace MiniStore.Application.DTOs.Purchases;

public sealed class SupplierPaymentCreatePageDto
{
    public string SupplierName { get; init; } = string.Empty;
    public string CurrencyCode { get; init; } = string.Empty;
    public CreateSupplierPaymentDto Form { get; init; } = new();
    public List<SupplierPaymentOpenBillDto> OpenBills { get; init; } = [];
    public List<SupplierPaymentMethodDto> PaymentMethods { get; init; } = [];
}

public sealed class SupplierPaymentOpenBillDto
{
    public int VendorBillId { get; init; }
    public string BillNumber { get; init; } = string.Empty;
    public string SupplierInvoiceNumber { get; init; } = string.Empty;
    public DateOnly BillDate { get; init; }
    public decimal TotalAmount { get; init; }
    public decimal PaidAmount { get; init; }
    public decimal OutstandingAmount { get; init; }
}

public sealed class SupplierPaymentMethodDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
}

public sealed class CreateSupplierPaymentDto
{
    [Range(1, int.MaxValue)]
    public int SourceVendorBillId { get; set; }
    [Range(1, int.MaxValue)]
    public int PaymentMethodId { get; set; }
    public DateOnly PaymentDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    [StringLength(100)]
    public string? ExternalReference { get; set; }
    [StringLength(1000)]
    public string? Notes { get; set; }
    [MinLength(1)]
    public List<CreateSupplierPaymentLineDto> Lines { get; set; } = [];
}

public sealed class CreateSupplierPaymentLineDto
{
    [Range(1, int.MaxValue)]
    public int VendorBillId { get; set; }
    [Range(typeof(decimal), "0", "999999999999", ParseLimitsInInvariantCulture = true, ConvertValueInInvariantCulture = true)]
    public decimal Amount { get; set; }
}

public sealed class SupplierPaymentDto
{
    public int Id { get; init; }
    public string PaymentNumber { get; init; } = string.Empty;
    public string SupplierName { get; init; } = string.Empty;
    public string PaymentMethodName { get; init; } = string.Empty;
    public DateOnly PaymentDate { get; init; }
    public string CurrencyCode { get; init; } = string.Empty;
    public string CompanyCurrencyCode { get; init; } = string.Empty;
    public decimal ExchangeRate { get; init; }
    public DateOnly ExchangeRateDate { get; init; }
    public string ExchangeRateSource { get; init; } = string.Empty;
    public string? ExternalReference { get; init; }
    public string? Notes { get; init; }
    public SupplierPaymentStatus Status { get; init; }
    public decimal TotalAmount { get; init; }
    public decimal CompanyCurrencyAmount { get; init; }
    public decimal PayableCarryingAmount { get; init; }
    public List<SupplierPaymentLineDto> Lines { get; init; } = [];
}

public sealed class SupplierPaymentLineDto
{
    public int VendorBillId { get; init; }
    public string BillNumber { get; init; } = string.Empty;
    public string SupplierInvoiceNumber { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public decimal CompanyCurrencyAmount { get; init; }
    public decimal PayableCarryingAmount { get; init; }
}
