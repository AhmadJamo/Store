using MiniStore.Application.DTOs.Settings;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public class GeneralSettingsService
{
    private readonly IGeneralSettingsRepository _repository;
    private readonly SemanticAuditEventService _audit;

    public GeneralSettingsService(
        IGeneralSettingsRepository repository,
        SemanticAuditEventService audit)
    {
        _repository = repository;
        _audit = audit;
    }

    public async Task<GeneralSettingsDto> GetAsync()
    {
        var settings = await _repository.GetAsync();
        if (settings == null)
        {
            return new GeneralSettingsDto();
        }

        return new GeneralSettingsDto
        {
            CompanyName = settings.CompanyName,
            CompanyNameArabic = settings.CompanyNameArabic,
            LegalName = settings.LegalName,
            LegalNameArabic = settings.LegalNameArabic,
            CommercialRegistrationNumber = settings.CommercialRegistrationNumber,
            TaxRegistrationNumber = settings.TaxRegistrationNumber,
            Email = settings.Email,
            Website = settings.Website,
            Phone = settings.Phone,
            SecondaryPhone = settings.SecondaryPhone,
            Address = settings.Address,
            AddressArabic = settings.AddressArabic,
            City = settings.City,
            CityArabic = settings.CityArabic,
            StateOrProvince = settings.StateOrProvince,
            PostalCode = settings.PostalCode,
            CountryCode = settings.CountryCode,
            DocumentNameStyle = settings.DocumentNameStyle,
            Currency = settings.Currency,
            DecimalPlaces = settings.DecimalPlaces,
            QuantityDecimalPlaces = settings.QuantityDecimalPlaces,
            DateFormat = settings.DateFormat,
            TimeZone = settings.TimeZone,
            DefaultLanguage = settings.DefaultLanguage,
            RowVersion = settings.RowVersion
        };
    }

    public async Task<GeneralSettingsPreferencesDto> GetPreferencesAsync()
    {
        var settings = await _repository.GetAsync();
        return settings is null
            ? new GeneralSettingsPreferencesDto()
            : new GeneralSettingsPreferencesDto
            {
                Currency = settings.Currency,
                DecimalPlaces = settings.DecimalPlaces,
                QuantityDecimalPlaces = settings.QuantityDecimalPlaces,
                DateFormat = settings.DateFormat,
                TimeZone = settings.TimeZone,
                DefaultLanguage = settings.DefaultLanguage,
                RowVersion = settings.RowVersion
            };
    }

    public async Task<OrganizationIdentityDto> GetOrganizationIdentityAsync()
    {
        var settings = await _repository.GetIdentityAsync();
        if (settings is null) return new OrganizationIdentityDto();
        return new OrganizationIdentityDto
        {
            TradeName = settings.CompanyName,
            TradeNameArabic = settings.CompanyNameArabic,
            LegalName = settings.LegalName,
            LegalNameArabic = settings.LegalNameArabic,
            CommercialRegistrationNumber = settings.CommercialRegistrationNumber,
            TaxRegistrationNumber = settings.TaxRegistrationNumber,
            Email = settings.Email,
            Website = settings.Website,
            Phone = settings.Phone,
            SecondaryPhone = settings.SecondaryPhone,
            Address = settings.Address,
            AddressArabic = settings.AddressArabic,
            City = settings.City,
            CityArabic = settings.CityArabic,
            StateOrProvince = settings.StateOrProvince,
            PostalCode = settings.PostalCode,
            CountryCode = settings.CountryCode,
            DocumentNameStyle = settings.DocumentNameStyle,
            HasLogo = settings.Logo?.Content is { Length: > 0 },
            LogoVersion = settings.Logo is null ? null : Convert.ToHexString(settings.Logo.RowVersion),
            RowVersion = settings.RowVersion,
            CustomFields = settings.IdentityFields.OrderBy(x => x.Sequence).Select(MapField).ToList()
        };
    }

    public async Task UpdatePreferencesAsync(GeneralSettingsPreferencesDto dto)
    {
        var settings = await _repository.GetAsync() ??
            throw new InvalidOperationException("Configure organization identity before saving general settings.");
        EnsureVersion(settings.RowVersion, dto.RowVersion);
        var before = AuditValues(settings);
        settings.SetCurrency(dto.Currency);
        settings.SetDecimalPlaces(dto.DecimalPlaces);
        settings.SetQuantityDecimalPlaces(dto.QuantityDecimalPlaces);
        settings.SetDateFormat(dto.DateFormat);
        settings.SetTimeZone(dto.TimeZone);
        settings.SetDefaultLanguage(dto.DefaultLanguage);
        await RecordAndSaveAsync(before, AuditValues(dto));
    }

    public async Task UpdateOrganizationIdentityAsync(OrganizationIdentityDto dto,
        OrganizationLogoUploadDto? logo, bool removeLogo)
    {
        var settings = await _repository.GetIdentityAsync();
        IReadOnlyDictionary<string, object?>? before = settings is null ? null : IdentityAuditValues(settings);
        if (settings is null)
        {
            settings = new GeneralSettings(dto.TradeName, dto.TradeNameArabic, dto.Phone, dto.Address,
                "JOD", 2, 3, "dd/MM/yyyy", "Asia/Amman");
            await _repository.AddAsync(settings);
        }
        else
        {
            EnsureVersion(settings.RowVersion, dto.RowVersion);
        }

        settings.SetOrganizationIdentity(dto.TradeName, dto.TradeNameArabic, dto.LegalName,
            dto.LegalNameArabic, dto.CommercialRegistrationNumber, dto.TaxRegistrationNumber,
            dto.Email, dto.Website, dto.Phone, dto.SecondaryPhone, dto.Address, dto.AddressArabic,
            dto.City, dto.CityArabic, dto.StateOrProvince, dto.PostalCode, dto.CountryCode,
            dto.DocumentNameStyle);

        var fields = dto.CustomFields
            .Where(x => !string.IsNullOrWhiteSpace(x.Label) || !string.IsNullOrWhiteSpace(x.Value))
            .Select((x, index) => new OrganizationIdentityField(x.Sequence > 0 ? x.Sequence : index + 1,
                x.Section, x.Label, x.LabelArabic, x.Value, x.ValueArabic, ToScope(x)))
            .ToList();
        settings.ReplaceIdentityFields(fields);

        if (logo is not null)
        {
            var contentType = DetectLogoContentType(logo.Content);
            settings.SetLogo(logo.Content, contentType, Path.GetFileName(logo.FileName));
        }
        else if (removeLogo)
        {
            settings.RemoveLogo();
        }

        await RecordAndSaveAsync(before, IdentityAuditValues(settings));
    }

    public async Task<OrganizationLogoDto?> GetOrganizationLogoAsync()
    {
        var logo = await _repository.GetLogoAsync();
        if (logo?.Content is not { Length: > 0 } content)
            return null;
        return new OrganizationLogoDto(content.ToArray(), logo.ContentType,
            logo.FileName, Convert.ToHexString(logo.RowVersion));
    }

    private static IReadOnlyDictionary<string, object?> AuditValues(GeneralSettings settings) =>
        new Dictionary<string, object?>
        {
            ["Currency"] = settings.Currency,
            ["DecimalPlaces"] = settings.DecimalPlaces,
            ["QuantityDecimalPlaces"] = settings.QuantityDecimalPlaces,
            ["DateFormat"] = settings.DateFormat,
            ["TimeZone"] = settings.TimeZone,
            ["DefaultLanguage"] = settings.DefaultLanguage
        };

    private static IReadOnlyDictionary<string, object?> AuditValues(GeneralSettingsPreferencesDto value) =>
        new Dictionary<string, object?>
        {
            ["Currency"] = value.Currency,
            ["DecimalPlaces"] = value.DecimalPlaces,
            ["QuantityDecimalPlaces"] = value.QuantityDecimalPlaces,
            ["DateFormat"] = value.DateFormat,
            ["TimeZone"] = value.TimeZone,
            ["DefaultLanguage"] = value.DefaultLanguage
        };

    private async Task RecordAndSaveAsync(IReadOnlyDictionary<string, object?>? before,
        IReadOnlyDictionary<string, object?> after)
    {
        await _audit.RecordTenantSuccessAsync(
            AuditEventCategory.Configuration,
            SemanticAuditEventTypes.GeneralSettingsChanged,
            "Update",
            "GeneralSettings",
            "Current",
            before,
            after);
        await _repository.SaveChangesAsync();
    }

    private static IReadOnlyDictionary<string, object?> IdentityAuditValues(GeneralSettings settings) =>
        new Dictionary<string, object?>
        {
            ["DocumentNameStyle"] = settings.DocumentNameStyle,
            ["HasLogo"] = settings.Logo?.Content is { Length: > 0 },
            ["CustomFieldCount"] = settings.IdentityFields.Count,
            ["SalesDocumentFieldCount"] = settings.IdentityFields.Count(x =>
                x.DocumentScope.HasFlag(OrganizationIdentityDocumentScope.Sales)),
            ["PurchaseDocumentFieldCount"] = settings.IdentityFields.Count(x =>
                x.DocumentScope.HasFlag(OrganizationIdentityDocumentScope.Purchasing))
        };

    private static OrganizationIdentityFieldDto MapField(OrganizationIdentityField value) => new()
    {
        Sequence = value.Sequence,
        Section = value.Section,
        Label = value.Label,
        LabelArabic = value.LabelArabic,
        Value = value.Value,
        ValueArabic = value.ValueArabic,
        ShowOnSalesDocuments = value.DocumentScope.HasFlag(OrganizationIdentityDocumentScope.Sales),
        ShowOnPurchaseDocuments = value.DocumentScope.HasFlag(OrganizationIdentityDocumentScope.Purchasing),
        ShowOnReceipts = value.DocumentScope.HasFlag(OrganizationIdentityDocumentScope.Receipts),
        ShowInFooter = value.DocumentScope.HasFlag(OrganizationIdentityDocumentScope.Footer)
    };

    private static OrganizationIdentityDocumentScope ToScope(OrganizationIdentityFieldDto value)
    {
        var scope = OrganizationIdentityDocumentScope.None;
        if (value.ShowOnSalesDocuments) scope |= OrganizationIdentityDocumentScope.Sales;
        if (value.ShowOnPurchaseDocuments) scope |= OrganizationIdentityDocumentScope.Purchasing;
        if (value.ShowOnReceipts) scope |= OrganizationIdentityDocumentScope.Receipts;
        if (value.ShowInFooter) scope |= OrganizationIdentityDocumentScope.Footer;
        return scope;
    }

    private static string DetectLogoContentType(byte[] content)
    {
        if (content.Length >= 8 && content[0] == 0x89 && content[1] == 0x50 && content[2] == 0x4E &&
            content[3] == 0x47 && content[4] == 0x0D && content[5] == 0x0A && content[6] == 0x1A && content[7] == 0x0A)
            return "image/png";
        if (content.Length >= 4 && content[0] == 0xFF && content[1] == 0xD8 && content[2] == 0xFF)
            return "image/jpeg";
        throw new ArgumentException("Organization logo must be a PNG or JPEG image.");
    }

    private static void EnsureVersion(byte[] current, byte[] submitted)
    {
        if (submitted.Length == 0 || !current.SequenceEqual(submitted))
            throw new SettingsConcurrencyException(
                "The general settings were modified by another user. Please reload the page and try again.");
    }
}
