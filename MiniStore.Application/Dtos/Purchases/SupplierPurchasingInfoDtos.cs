using System.ComponentModel.DataAnnotations;

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

public sealed class SupplierPurchasingInfoPageDto
{
    public List<SupplierPurchasingInfoDto> Rows { get; init; } = [];
    public List<SupplierPurchasingOptionDto> Suppliers { get; init; } = [];
    public List<SupplierPurchasingOptionDto> Products { get; init; } = [];
    public List<SupplierPurchasingOptionDto> Units { get; init; } = [];
    public SaveSupplierPurchasingInfoDto Form { get; init; } = new();
    public string BaseCurrency { get; init; } = "JOD";
}
