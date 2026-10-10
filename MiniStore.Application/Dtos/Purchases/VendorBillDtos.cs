using MiniStore.Domain.Entities;

namespace MiniStore.Application.DTOs.Purchases;

public sealed class VendorBillCreatePageDto
{
    public int PurchaseOrderId { get; init; }
    public string PurchaseOrderNumber { get; init; } = string.Empty;
    public string SupplierName { get; init; } = string.Empty;
    public string CurrencyCode { get; init; } = string.Empty;
    public CreateVendorBillDto Form { get; init; } = new();
    public List<VendorBillAvailableLineDto> Lines { get; init; } = [];
    public List<VendorBillTaxRateDto> TaxRates { get; init; } = [];
}

public sealed class VendorBillAvailableLineDto
{
    public int GoodsReceiptLineId { get; init; }
    public string ReceiptNumber { get; init; } = string.Empty;
    public DateOnly ReceiptDate { get; init; }
    public string ProductCode { get; init; } = string.Empty;
    public string ProductName { get; init; } = string.Empty;
    public string UnitName { get; init; } = string.Empty;
    public decimal RemainingQuantity { get; init; }
    public decimal SuggestedUnitPrice { get; init; }
    public int? SuggestedTaxRateId { get; init; }
}

public sealed class VendorBillTaxRateDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal Rate { get; init; }
    public bool IsPriceInclusive { get; init; }
}

public sealed class CreateVendorBillDto
{
    public int PurchaseOrderId { get; set; }
    public string SupplierInvoiceNumber { get; set; } = string.Empty;
    public DateOnly BillDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    public string? Notes { get; set; }
    public List<CreateVendorBillLineDto> Lines { get; set; } = [];
}

public sealed class CreateVendorBillLineDto
{
    public int GoodsReceiptLineId { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public int? TaxRateId { get; set; }
}

public sealed class VendorBillDto
{
    public int Id { get; init; }
    public string BillNumber { get; init; } = string.Empty;
    public string PurchaseOrderNumber { get; init; } = string.Empty;
    public string SupplierName { get; init; } = string.Empty;
    public string SupplierInvoiceNumber { get; init; } = string.Empty;
    public DateOnly BillDate { get; init; }
    public string CurrencyCode { get; init; } = string.Empty;
    public string? Notes { get; init; }
    public VendorBillStatus Status { get; init; }
    public decimal NetAmount { get; init; }
    public decimal TaxAmount { get; init; }
    public decimal TotalAmount { get; init; }
    public decimal ReceiptClearingAmount { get; init; }
    public List<VendorBillLineDto> Lines { get; init; } = [];
}

public sealed class VendorBillLineDto
{
    public string ProductCode { get; init; } = string.Empty;
    public string ProductName { get; init; } = string.Empty;
    public string UnitName { get; init; } = string.Empty;
    public decimal Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal TaxPercent { get; init; }
    public bool IsTaxInclusive { get; init; }
    public decimal NetAmount { get; init; }
    public decimal TaxAmount { get; init; }
    public decimal GrossAmount { get; init; }
    public decimal ReceiptClearingAmount { get; init; }
}
