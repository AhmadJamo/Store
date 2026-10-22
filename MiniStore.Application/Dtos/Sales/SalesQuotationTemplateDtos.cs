using System.ComponentModel.DataAnnotations;
using MiniStore.Domain.Entities;

namespace MiniStore.Application.Dtos.Sales;

public sealed class SalesQuotationTemplatePageDto
{
    public CreateSalesQuotationTemplateDto Form { get; init; } = new();
    public List<SalesQuotationOptionDto> Products { get; init; } = [];
    public List<SalesQuotationOptionDto> Units { get; init; } = [];
    public List<SalesQuotationOptionDto> PriceLists { get; init; } = [];
    public List<SalesQuotationOptionDto> TaxRates { get; init; } = [];
}

public sealed class CreateSalesQuotationTemplateDto
{
    public int? SourceTemplateId { get; set; }
    [Required, StringLength(50)] public string Code { get; set; } = "QUOTE";
    [Range(1, 9999)] public int Version { get; set; } = 1;
    [Required, StringLength(150)] public string Name { get; set; } = string.Empty;
    [StringLength(1000)] public string? Description { get; set; }
    [StringLength(100)] public string? Category { get; set; }
    [StringLength(500)] public string? Tags { get; set; }
    [Range(0, 3650)] public int ValidityDays { get; set; } = 30;
    public SaleChannel Channel { get; set; } = SaleChannel.Wholesale;
    public int? SalesPriceListId { get; set; }
    public bool AllowAdditionalLines { get; set; } = true;
    public bool IsDefault { get; set; }
    [StringLength(4000)] public string? Introduction { get; set; }
    [StringLength(8000)] public string? TermsAndConditions { get; set; }
    [StringLength(4000)] public string? FooterNotes { get; set; }
    public List<CreateSalesQuotationTemplateItemDto> Items { get; set; } = [];
}

public sealed class CreateSalesQuotationTemplateItemDto
{
    [Range(1, 9999)] public int Sequence { get; set; }
    [Required, StringLength(150)] public string SectionTitle { get; set; } = string.Empty;
    [Range(1, int.MaxValue)] public int ProductId { get; set; }
    [Range(1, int.MaxValue)] public int MeasurementUnitId { get; set; }
    [Range(typeof(decimal), "0.000001", "999999999999")] public decimal DefaultQuantity { get; set; } = 1;
    [Range(typeof(decimal), "0.000001", "999999999999")] public decimal MinimumQuantity { get; set; } = 1;
    [Range(typeof(decimal), "0.000001", "999999999999")] public decimal MaximumQuantity { get; set; } = 1;
    public bool AllowQuantityChange { get; set; }
    public int? TaxRateId { get; set; }
    public SalesQuotationTemplateItemType ItemType { get; set; } = SalesQuotationTemplateItemType.Required;
    public bool IsDefaultSelected { get; set; } = true;
    [StringLength(50)] public string? OptionGroupCode { get; set; }
    [StringLength(150)] public string? OptionGroupName { get; set; }
    [Range(0, 100)] public int GroupMinimumSelections { get; set; }
    [Range(0, 100)] public int GroupMaximumSelections { get; set; }
    [StringLength(500)] public string? Notes { get; set; }
}

public sealed record SalesQuotationTemplateSummaryDto(int Id, string Code, int Version, string Name,
    string? Category, string? Tags, int ItemCount, int OptionalItemCount, int ValidityDays,
    bool IsDefault, SalesQuotationTemplateStatus Status, byte[] RowVersion);

public sealed class SalesQuotationTemplateDetailsDto
{
    public int Id { get; init; } public string Code { get; init; } = ""; public int Version { get; init; }
    public string Name { get; init; } = ""; public string? Description { get; init; } public string? Category { get; init; }
    public string? Tags { get; init; } public int ValidityDays { get; init; } public SaleChannel Channel { get; init; }
    public string? PriceListName { get; init; } public bool AllowAdditionalLines { get; init; } public bool IsDefault { get; init; }
    public string? Introduction { get; init; } public string? TermsAndConditions { get; init; } public string? FooterNotes { get; init; }
    public SalesQuotationTemplateStatus Status { get; init; } public string CreatedByUserId { get; init; } = "";
    public DateTime CreatedAtUtc { get; init; } public DateTime? ActivatedAtUtc { get; init; } public byte[] RowVersion { get; init; } = [];
    public List<SalesQuotationTemplateItemDto> Items { get; init; } = [];
}

public sealed record SalesQuotationTemplateItemDto(int Sequence, string SectionTitle, int ProductId,
    string ProductName, int MeasurementUnitId, string UnitName, decimal DefaultQuantity,
    decimal MinimumQuantity, decimal MaximumQuantity, bool AllowQuantityChange, int? TaxRateId,
    SalesQuotationTemplateItemType ItemType, bool IsDefaultSelected, string? OptionGroupCode,
    string? OptionGroupName, int GroupMinimumSelections, int GroupMaximumSelections, string? Notes);

public sealed class SalesQuotationTemplateCommandDto
{
    [Range(1, int.MaxValue)] public int Id { get; set; }
    public byte[] RowVersion { get; set; } = [];
}
