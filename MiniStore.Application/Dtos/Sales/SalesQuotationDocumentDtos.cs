using MiniStore.Domain.Entities;

namespace MiniStore.Application.Dtos.Sales;

public sealed record SalesQuotationDocumentSnapshotDto(
    int SchemaVersion,
    SalesQuotationDocumentOrganizationDto Organization,
    SalesQuotationDocumentDesignDto Design,
    SalesQuotationDocumentContentDto Quotation);

public sealed record SalesQuotationDocumentOrganizationDto(
    string TradeName, string? TradeNameArabic, string? LegalName, string? LegalNameArabic,
    OrganizationDocumentNameStyle DocumentNameStyle, string? RegistrationNumber, string? TaxNumber,
    string? Email, string? Website, string? Phone, string? SecondaryPhone,
    string? Address, string? AddressArabic, string? City, string? CityArabic,
    string? StateOrProvince, string? PostalCode, string? CountryCode,
    IReadOnlyList<SalesQuotationDocumentIdentityFieldDto> CustomFields);

public sealed record SalesQuotationDocumentIdentityFieldDto(
    int Sequence, string Label, string? LabelArabic, string Value, string? ValueArabic,
    OrganizationIdentityFieldSection Section, OrganizationIdentityDocumentScope Scope);

public sealed record SalesQuotationDocumentDesignDto(
    string Code, int Version, string Name, DocumentLanguage DefaultLanguage, bool AllowLanguageOverride,
    DocumentPageSize PageSize, DocumentPageOrientation Orientation, decimal? CustomPageWidthMm,
    decimal? CustomPageHeightMm, decimal MarginTopMm, decimal MarginEndMm, decimal MarginBottomMm,
    decimal MarginStartMm, string PrimaryColor, string AccentColor, string TextColor,
    string TableHeaderTextColor, DocumentFontFamily FontFamily, DocumentHeaderLayout HeaderLayout,
    bool UseOrganizationLogo, DocumentElementPlacement LogoPlacement, int LogoMaximumHeightPx,
    DocumentElementPlacement OrganizationNamePlacement, bool ShowLegalName, bool ShowTradeName,
    bool ShowRegistrationNumber, bool ShowTaxNumber, bool ShowAddress, bool ShowContactDetails,
    bool ShowCustomIdentityFields, bool ShowPageNumbers, bool ShowStatusWatermark, bool ShowProductCode,
    bool ShowUnit, bool ShowLineNotes, bool ShowTaxDetails, bool ShowNetTotal, bool ShowTaxTotal,
    string? CoverTitle, string? CoverTitleArabic, string? CoverSubtitle, string? CoverSubtitleArabic,
    string? HeaderText, string? HeaderTextArabic, string? FooterText, string? FooterTextArabic,
    string? WatermarkText, string? WatermarkTextArabic, decimal WatermarkOpacity,
    IReadOnlyList<SalesQuotationDocumentSectionDto> Sections);

public sealed record SalesQuotationDocumentSectionDto(
    int Sequence, DocumentDesignSectionType SectionType, bool IsVisible, bool PageBreakBefore,
    string Title, string? TitleArabic, string? Content, string? ContentArabic);

public sealed record SalesQuotationDocumentContentDto(
    string Number, SalesQuotationStatus Status, string CustomerCode, string CustomerName,
    string? CustomerLegalName, string? CustomerTaxNumber, string? CustomerAddress,
    DateOnly QuotationDate, DateOnly ValidUntil, string CurrencyCode, decimal ExchangeRate,
    DateOnly ExchangeRateDate, string ExchangeRateSource, int PaymentTermDays,
    string? Introduction, string? TermsAndConditions, string? FooterNotes, string? Notes,
    decimal NetAmount, decimal TaxAmount, decimal TotalAmount,
    IReadOnlyList<SalesQuotationDocumentLineDto> Lines);

public sealed record SalesQuotationDocumentLineDto(
    string ProductCode, string ProductName, string UnitCode, string UnitName, decimal Quantity,
    decimal UnitPrice, decimal NetAmount, string? TaxName, decimal? TaxRatePercent,
    decimal TaxAmount, decimal TotalAmount, string? Notes);

public sealed record SalesQuotationPdfRequest(
    SalesQuotationDocumentSnapshotDto Snapshot, byte[]? LogoContent, string? LogoContentType,
    DocumentLanguage? LanguageOverride);

public sealed record SalesQuotationPdfResult(byte[] Content, string FileName);
