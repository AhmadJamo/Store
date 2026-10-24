using System.ComponentModel.DataAnnotations;
using MiniStore.Domain.Entities;

namespace MiniStore.Application.Dtos.Settings;

public sealed class CreateDocumentDesignDto
{
    public int? SourceDesignId { get; set; }
    public DocumentDesignType DocumentType { get; set; } = DocumentDesignType.SalesQuotation;
    [Required, StringLength(50)] public string Code { get; set; } = "STANDARD";
    [Range(1, 9999)] public int Version { get; set; } = 1;
    [Required, StringLength(150)] public string Name { get; set; } = string.Empty;
    [StringLength(1000)] public string? Description { get; set; }
    public bool IsDefault { get; set; }
    public DocumentLanguage DefaultLanguage { get; set; } = DocumentLanguage.Bilingual;
    public bool AllowLanguageOverride { get; set; } = true;
    public DocumentPageSize PageSize { get; set; } = DocumentPageSize.A4;
    public DocumentPageOrientation Orientation { get; set; } = DocumentPageOrientation.Portrait;
    [Range(typeof(decimal), "50", "1000", ParseLimitsInInvariantCulture = true, ConvertValueInInvariantCulture = true)] public decimal? CustomPageWidthMm { get; set; }
    [Range(typeof(decimal), "50", "1000", ParseLimitsInInvariantCulture = true, ConvertValueInInvariantCulture = true)] public decimal? CustomPageHeightMm { get; set; }
    [Range(typeof(decimal), "0", "50", ParseLimitsInInvariantCulture = true, ConvertValueInInvariantCulture = true)] public decimal MarginTopMm { get; set; } = 12;
    [Range(typeof(decimal), "0", "50", ParseLimitsInInvariantCulture = true, ConvertValueInInvariantCulture = true)] public decimal MarginEndMm { get; set; } = 12;
    [Range(typeof(decimal), "0", "50", ParseLimitsInInvariantCulture = true, ConvertValueInInvariantCulture = true)] public decimal MarginBottomMm { get; set; } = 15;
    [Range(typeof(decimal), "0", "50", ParseLimitsInInvariantCulture = true, ConvertValueInInvariantCulture = true)] public decimal MarginStartMm { get; set; } = 12;
    [Required, RegularExpression("^#[0-9A-Fa-f]{6}$")] public string PrimaryColor { get; set; } = "#1F4E78";
    [Required, RegularExpression("^#[0-9A-Fa-f]{6}$")] public string AccentColor { get; set; } = "#D9EAF7";
    [Required, RegularExpression("^#[0-9A-Fa-f]{6}$")] public string TextColor { get; set; } = "#1F2937";
    [Required, RegularExpression("^#[0-9A-Fa-f]{6}$")] public string TableHeaderTextColor { get; set; } = "#FFFFFF";
    public DocumentFontFamily FontFamily { get; set; } = DocumentFontFamily.NotoSans;
    public DocumentHeaderLayout HeaderLayout { get; set; } = DocumentHeaderLayout.LogoStartIdentityEnd;
    public bool UseOrganizationLogo { get; set; } = true;
    public DocumentElementPlacement LogoPlacement { get; set; } = DocumentElementPlacement.Start;
    [Range(24, 300)] public int LogoMaximumHeightPx { get; set; } = 72;
    public DocumentElementPlacement OrganizationNamePlacement { get; set; } = DocumentElementPlacement.End;
    public bool ShowLegalName { get; set; } = true;
    public bool ShowTradeName { get; set; } = true;
    public bool ShowRegistrationNumber { get; set; } = true;
    public bool ShowTaxNumber { get; set; } = true;
    public bool ShowAddress { get; set; } = true;
    public bool ShowContactDetails { get; set; } = true;
    public bool ShowCustomIdentityFields { get; set; } = true;
    public bool ShowPageNumbers { get; set; } = true;
    public bool ShowStatusWatermark { get; set; } = true;
    public bool ShowProductCode { get; set; } = true;
    public bool ShowUnit { get; set; } = true;
    public bool ShowLineNotes { get; set; } = true;
    public bool ShowTaxDetails { get; set; } = true;
    public bool ShowNetTotal { get; set; } = true;
    public bool ShowTaxTotal { get; set; } = true;
    public bool ShowQrCode { get; set; }
    public bool ShowDocumentBarcode { get; set; }
    [StringLength(250)] public string? CoverTitle { get; set; }
    [StringLength(250)] public string? CoverTitleArabic { get; set; }
    [StringLength(1000)] public string? CoverSubtitle { get; set; }
    [StringLength(1000)] public string? CoverSubtitleArabic { get; set; }
    [StringLength(500)] public string? HeaderText { get; set; }
    [StringLength(500)] public string? HeaderTextArabic { get; set; }
    [StringLength(1000)] public string? FooterText { get; set; }
    [StringLength(1000)] public string? FooterTextArabic { get; set; }
    [StringLength(100)] public string? WatermarkText { get; set; }
    [StringLength(100)] public string? WatermarkTextArabic { get; set; }
    [Range(typeof(decimal), "0.02", "0.5", ParseLimitsInInvariantCulture = true, ConvertValueInInvariantCulture = true)] public decimal WatermarkOpacity { get; set; } = 0.12m;
    public List<CreateDocumentDesignSectionDto> Sections { get; set; } = DefaultSections();

    public static List<CreateDocumentDesignSectionDto> DefaultSections() =>
    [
        new() { Sequence = 10, SectionType = DocumentDesignSectionType.OrganizationIdentity, Title = "Organization", TitleArabic = "المنشأة" },
        new() { Sequence = 20, SectionType = DocumentDesignSectionType.DocumentDetails, Title = "Document details", TitleArabic = "بيانات المستند" },
        new() { Sequence = 30, SectionType = DocumentDesignSectionType.PartyDetails, Title = "Customer / supplier", TitleArabic = "العميل / المورد" },
        new() { Sequence = 40, SectionType = DocumentDesignSectionType.Introduction, Title = "Introduction", TitleArabic = "المقدمة" },
        new() { Sequence = 50, SectionType = DocumentDesignSectionType.LineItems, Title = "Items", TitleArabic = "البنود" },
        new() { Sequence = 60, SectionType = DocumentDesignSectionType.Totals, Title = "Totals", TitleArabic = "الإجماليات" },
        new() { Sequence = 70, SectionType = DocumentDesignSectionType.TermsAndConditions, Title = "Terms and conditions", TitleArabic = "الشروط والأحكام" },
        new() { Sequence = 80, SectionType = DocumentDesignSectionType.Notes, Title = "Notes", TitleArabic = "ملاحظات" },
        new() { Sequence = 90, SectionType = DocumentDesignSectionType.Acceptance, Title = "Acceptance", TitleArabic = "الاعتماد" },
        new() { Sequence = 100, SectionType = DocumentDesignSectionType.Footer, Title = "Footer", TitleArabic = "التذييل" }
    ];

    public DocumentDesignSettings ToSettings() => new(DefaultLanguage, AllowLanguageOverride, PageSize, Orientation,
        CustomPageWidthMm, CustomPageHeightMm, MarginTopMm, MarginEndMm, MarginBottomMm, MarginStartMm,
        PrimaryColor, AccentColor, TextColor, TableHeaderTextColor, FontFamily, HeaderLayout,
        UseOrganizationLogo, LogoPlacement, LogoMaximumHeightPx, OrganizationNamePlacement,
        ShowLegalName, ShowTradeName, ShowRegistrationNumber, ShowTaxNumber, ShowAddress, ShowContactDetails,
        ShowCustomIdentityFields, ShowPageNumbers, ShowStatusWatermark, ShowProductCode, ShowUnit,
        ShowLineNotes, ShowTaxDetails, ShowNetTotal, ShowTaxTotal, ShowQrCode, ShowDocumentBarcode,
        CoverTitle, CoverTitleArabic, CoverSubtitle, CoverSubtitleArabic, HeaderText, HeaderTextArabic,
        FooterText, FooterTextArabic, WatermarkText, WatermarkTextArabic, WatermarkOpacity);
}

public sealed class CreateDocumentDesignSectionDto
{
    [Range(1, 1000)] public int Sequence { get; set; }
    public DocumentDesignSectionType SectionType { get; set; }
    public bool IsVisible { get; set; } = true;
    public bool PageBreakBefore { get; set; }
    [Required, StringLength(150)] public string Title { get; set; } = string.Empty;
    [StringLength(150)] public string? TitleArabic { get; set; }
    [StringLength(4000)] public string? Content { get; set; }
    [StringLength(4000)] public string? ContentArabic { get; set; }
}

public sealed record DocumentDesignSummaryDto(int Id, DocumentDesignType DocumentType, string Code, int Version,
    string Name, DocumentLanguage DefaultLanguage, DocumentPageSize PageSize,
    DocumentPageOrientation Orientation, string PrimaryColor, int SectionCount,
    bool IsDefault, DocumentDesignStatus Status, byte[] RowVersion);

public sealed class DocumentDesignDetailsDto
{
    public int Id { get; init; }
    public CreateDocumentDesignDto Design { get; init; } = new();
    public DocumentDesignStatus Status { get; init; }
    public string CreatedByUserId { get; init; } = string.Empty;
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? ActivatedAtUtc { get; init; }
    public byte[] RowVersion { get; init; } = [];
}

public sealed class DocumentDesignCommandDto
{
    [Range(1, int.MaxValue)] public int Id { get; set; }
    public byte[] RowVersion { get; set; } = [];
}
