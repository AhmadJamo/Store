namespace MiniStore.Application.DTOs.Settings;

using System.ComponentModel.DataAnnotations;
using MiniStore.Domain.Entities;

public class GeneralSettingsDto
{
    public string CompanyName { get; set; } = string.Empty;

    public string? CompanyNameArabic { get; set; }

    public string? LegalName { get; set; }
    public string? LegalNameArabic { get; set; }
    public string? CommercialRegistrationNumber { get; set; }
    public string? TaxRegistrationNumber { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public string? SecondaryPhone { get; set; }
    public string? AddressArabic { get; set; }
    public string? City { get; set; }
    public string? CityArabic { get; set; }
    public string? StateOrProvince { get; set; }
    public string? PostalCode { get; set; }
    public string? CountryCode { get; set; }
    public OrganizationDocumentNameStyle DocumentNameStyle { get; set; } = OrganizationDocumentNameStyle.TradeName;
    public bool HasLogo { get; set; }
    public string? LogoVersion { get; set; }

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public string Currency { get; set; } = "JOD";

    public int DecimalPlaces { get; set; } = 2;

    public int QuantityDecimalPlaces { get; set; } = 3;

    public string DateFormat { get; set; } = "dd/MM/yyyy";

    public string TimeZone { get; set; } = "Asia/Amman";

    public MiniStore.Domain.Entities.UiLanguage DefaultLanguage { get; set; } =
        MiniStore.Domain.Entities.UiLanguage.English;

    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}

public sealed class GeneralSettingsPreferencesDto
{
    [Required, StringLength(10)] public string Currency { get; set; } = "JOD";
    [Range(0, 4)] public int DecimalPlaces { get; set; } = 2;
    [Range(0, 4)] public int QuantityDecimalPlaces { get; set; } = 3;
    [Required, StringLength(50)] public string DateFormat { get; set; } = "dd/MM/yyyy";
    [Required, StringLength(100)] public string TimeZone { get; set; } = "Asia/Amman";
    public UiLanguage DefaultLanguage { get; set; } = UiLanguage.English;
    public byte[] RowVersion { get; set; } = [];
}

public sealed class OrganizationIdentityDto
{
    [Required, StringLength(200)] public string TradeName { get; set; } = string.Empty;
    [StringLength(200)] public string? TradeNameArabic { get; set; }
    [StringLength(200)] public string? LegalName { get; set; }
    [StringLength(200)] public string? LegalNameArabic { get; set; }
    [StringLength(100)] public string? CommercialRegistrationNumber { get; set; }
    [StringLength(100)] public string? TaxRegistrationNumber { get; set; }
    [EmailAddress, StringLength(254)] public string? Email { get; set; }
    [Url, StringLength(500)] public string? Website { get; set; }
    [StringLength(50)] public string? Phone { get; set; }
    [StringLength(50)] public string? SecondaryPhone { get; set; }
    [StringLength(500)] public string? Address { get; set; }
    [StringLength(500)] public string? AddressArabic { get; set; }
    [StringLength(100)] public string? City { get; set; }
    [StringLength(100)] public string? CityArabic { get; set; }
    [StringLength(100)] public string? StateOrProvince { get; set; }
    [StringLength(30)] public string? PostalCode { get; set; }
    [StringLength(2, MinimumLength = 2)] public string? CountryCode { get; set; }
    public OrganizationDocumentNameStyle DocumentNameStyle { get; set; } = OrganizationDocumentNameStyle.TradeName;
    public bool HasLogo { get; set; }
    public string? LogoVersion { get; set; }
    public byte[] RowVersion { get; set; } = [];
    public List<OrganizationIdentityFieldDto> CustomFields { get; set; } = [];
}

public sealed class OrganizationIdentityFieldDto
{
    [Range(1, 100)] public int Sequence { get; set; }
    public OrganizationIdentityFieldSection Section { get; set; } = OrganizationIdentityFieldSection.Other;
    [Required, StringLength(100)] public string Label { get; set; } = string.Empty;
    [StringLength(100)] public string? LabelArabic { get; set; }
    [Required, StringLength(500)] public string Value { get; set; } = string.Empty;
    [StringLength(500)] public string? ValueArabic { get; set; }
    public bool ShowOnSalesDocuments { get; set; } = true;
    public bool ShowOnPurchaseDocuments { get; set; }
    public bool ShowOnReceipts { get; set; }
    public bool ShowInFooter { get; set; }
}

public sealed record OrganizationLogoUploadDto(byte[] Content, string FileName);
public sealed record OrganizationLogoDto(byte[] Content, string ContentType, string FileName, string Version);
