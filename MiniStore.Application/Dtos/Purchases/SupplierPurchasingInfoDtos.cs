using System.ComponentModel.DataAnnotations;
using MiniStore.Domain.Entities;

namespace MiniStore.Application.DTOs.Purchases;

public sealed class SupplierPurchasingInfoDto
{
    public int Id { get; init; }
    public int SupplierId { get; init; }
    public string SupplierName { get; init; } = string.Empty;
    public int ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public string ProductCode { get; init; } = string.Empty;
    public int PurchaseMeasurementUnitId { get; init; }
    public string PurchaseUnit { get; init; } = string.Empty;
    public string SupplierProductCode { get; init; } = string.Empty;
    public string? SupplierDescription { get; init; }
    public decimal MinimumOrderQuantity { get; init; }
    public decimal OrderMultiple { get; init; }
    public int LeadTimeDays { get; init; }
    public decimal UnitPrice { get; init; }
    public string CurrencyCode { get; init; } = string.Empty;
    public DateOnly ValidFrom { get; init; }
    public DateOnly? ValidTo { get; init; }
    public bool IsPreferred { get; init; }
    public int Priority { get; init; }
    public bool IsActive { get; init; }
    public int PriceBreakCount { get; init; }
    public byte[] RowVersion { get; init; } = [];
}

public sealed class SaveSupplierPurchasingInfoDto
{
    public int Id { get; set; }
    public int SupplierId { get; set; }
    public int ProductId { get; set; }
    public int PurchaseMeasurementUnitId { get; set; }
    [StringLength(100)] public string SupplierProductCode { get; set; } = string.Empty;
    [StringLength(300)] public string? SupplierDescription { get; set; }
    public decimal MinimumOrderQuantity { get; set; } = 1;
    public decimal OrderMultiple { get; set; } = 1;
    public int LeadTimeDays { get; set; }
    public decimal UnitPrice { get; set; }
    [StringLength(3, MinimumLength = 3)] public string CurrencyCode { get; set; } = "JOD";
    public DateOnly ValidFrom { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    public DateOnly? ValidTo { get; set; }
    public bool IsPreferred { get; set; }
    public int Priority { get; set; } = 100;
    public byte[] RowVersion { get; set; } = [];
}

public sealed record SupplierPurchasingOptionDto(int Id, string Label);

public sealed class BulkSaveSupplierPurchasingInfoDto
{
    public int SupplierId { get; set; }
    public List<BulkSupplierPurchasingRowDto> Rows { get; set; } = [];
}

public sealed class BulkSupplierPurchasingRowDto
{
    public bool Selected { get; set; }
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductCode { get; init; } = string.Empty;
    public string ProductName { get; init; } = string.Empty;
    public int PurchaseMeasurementUnitId { get; set; }
    public List<SupplierPurchasingOptionDto> UnitOptions { get; init; } = [];
    [StringLength(100)] public string SupplierProductCode { get; set; } = string.Empty;
    [StringLength(300)] public string? SupplierDescription { get; set; }
    public decimal MinimumOrderQuantity { get; set; } = 1;
    public decimal OrderMultiple { get; set; } = 1;
    public int LeadTimeDays { get; set; }
    public decimal UnitPrice { get; set; }
    public string CurrencyCode { get; set; } = "JOD";
    public DateOnly ValidFrom { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    public DateOnly? ValidTo { get; set; }
    public bool IsPreferred { get; set; }
    public int Priority { get; set; } = 100;
    public bool IsActive { get; init; } = true;
    public byte[] RowVersion { get; set; } = [];
}

public sealed class SupplierPurchasingInfoPageDto
{
    public List<SupplierPurchasingInfoDto> Rows { get; init; } = [];
    public List<SupplierPurchasingOptionDto> Suppliers { get; init; } = [];
    public List<SupplierPurchasingOptionDto> Products { get; init; } = [];
    public List<SupplierPurchasingOptionDto> Units { get; init; } = [];
    public SaveSupplierPurchasingInfoDto Form { get; init; } = new();
    public int? SelectedSupplierId { get; init; }
    public BulkSaveSupplierPurchasingInfoDto BulkForm { get; init; } = new();
    public string BaseCurrency { get; init; } = "JOD";
}

public sealed class SupplierPurchasingCsvPreviewDto
{
    public int SupplierId { get; init; }
    public string SupplierName { get; init; } = string.Empty;
    public string FileName { get; init; } = string.Empty;
    public string CsvContent { get; init; } = string.Empty;
    public List<SupplierPurchasingCsvPreviewRowDto> Rows { get; init; } = [];
    public int ValidRowCount => Rows.Count(x => x.Errors.Count == 0);
    public int ErrorRowCount => Rows.Count(x => x.Errors.Count > 0);
    public bool CanImport => Rows.Count > 0 && ErrorRowCount == 0;
}

public sealed class SupplierPurchasingCsvPreviewRowDto
{
    public int RowNumber { get; init; }
    public string ChangeType { get; init; } = string.Empty;
    public string ProductCode { get; init; } = string.Empty;
    public string ProductName { get; init; } = string.Empty;
    public string PurchaseUnitCode { get; init; } = string.Empty;
    public string SupplierProductCode { get; init; } = string.Empty;
    public decimal? UnitPrice { get; init; }
    public string CurrencyCode { get; init; } = string.Empty;
    public List<string> Errors { get; init; } = [];
    public SaveSupplierPurchasingInfoDto? ValidatedInput { get; init; }
}

public sealed class ImportSupplierPurchasingCsvDto
{
    public int SupplierId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string CsvContent { get; set; } = string.Empty;
}

public sealed class SupplierPriceBreakPageDto
{
    public int PurchasingInfoId { get; init; }
    public int SupplierId { get; init; }
    public string SupplierName { get; init; } = string.Empty;
    public string ProductLabel { get; init; } = string.Empty;
    public string PurchaseUnit { get; init; } = string.Empty;
    public decimal MinimumOrderQuantity { get; init; }
    public decimal BaseUnitPrice { get; init; }
    public string CurrencyCode { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public SaveSupplierPriceBreaksDto Form { get; init; } = new();
}

public sealed class SaveSupplierPriceBreaksDto
{
    public int PurchasingInfoId { get; set; }
    public int SupplierId { get; set; }
    public byte[] RowVersion { get; set; } = [];
    public List<SaveSupplierPriceBreakRowDto> Rows { get; set; } = [];
}

public sealed class SaveSupplierPriceBreakRowDto
{
    public bool Selected { get; set; } = true;
    public decimal MinimumQuantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountPercent { get; set; }
}

public sealed class SupplierPriceHistoryPageDto
{
    public int PurchasingInfoId { get; init; }
    public int SupplierId { get; init; }
    public string SupplierName { get; init; } = string.Empty;
    public string ProductLabel { get; init; } = string.Empty;
    public string PurchaseUnit { get; init; } = string.Empty;
    public List<SupplierPriceHistoryRowDto> Rows { get; init; } = [];
}

public sealed class SupplierPriceHistoryRowDto
{
    public int Revision { get; init; }
    public SupplierPriceChangeSource Source { get; init; }
    public decimal MinimumOrderQuantity { get; init; }
    public decimal BaseUnitPrice { get; init; }
    public string CurrencyCode { get; init; } = string.Empty;
    public DateOnly ValidFrom { get; init; }
    public DateOnly? ValidTo { get; init; }
    public string ChangedByUserId { get; init; } = string.Empty;
    public DateTime ChangedAtUtc { get; init; }
    public List<SupplierPriceHistoryBreakDto> PriceBreaks { get; init; } = [];
}

public sealed class SupplierPriceHistoryBreakDto
{
    public decimal MinimumQuantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal DiscountPercent { get; init; }
    public decimal NetUnitPrice { get; init; }
}
