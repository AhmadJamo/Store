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
            Phone = settings.Phone,
            Address = settings.Address,
            Currency = settings.Currency,
            DecimalPlaces = settings.DecimalPlaces,
            QuantityDecimalPlaces = settings.QuantityDecimalPlaces,
            DateFormat = settings.DateFormat,
            TimeZone = settings.TimeZone,
            DefaultLanguage = settings.DefaultLanguage,
            RowVersion = settings.RowVersion
        };
    }

    public async Task UpdateAsync(
        GeneralSettingsDto dto)
    {
        var settings = await _repository.GetAsync();
        var before = settings is null ? null : AuditValues(settings);

        if (settings == null)
        {
            settings = new GeneralSettings(
                dto.CompanyName,
                dto.CompanyNameArabic,
                dto.Phone,
                dto.Address,
                dto.Currency,
                dto.DecimalPlaces,
                dto.QuantityDecimalPlaces,
                dto.DateFormat,
                dto.TimeZone,
                dto.DefaultLanguage);

            await _repository.AddAsync(settings);
        }
        else
        {

            if (!settings.RowVersion.SequenceEqual(dto.RowVersion))
            {
                throw new InvalidOperationException(
                    "The general settings were modified by another user. Please reload the page and try again.");
            }





            settings.SetCompanyName(dto.CompanyName);
            settings.SetCompanyNameArabic(dto.CompanyNameArabic);
            settings.SetPhone(dto.Phone);
            settings.SetAddress(dto.Address);
            settings.SetCurrency(dto.Currency);
            settings.SetDecimalPlaces(dto.DecimalPlaces);
            settings.SetQuantityDecimalPlaces(
                dto.QuantityDecimalPlaces);
            settings.SetDateFormat(dto.DateFormat);
            settings.SetTimeZone(dto.TimeZone);
            settings.SetDefaultLanguage(dto.DefaultLanguage);
        }

        await _audit.RecordTenantSuccessAsync(
            AuditEventCategory.Configuration,
            SemanticAuditEventTypes.GeneralSettingsChanged,
            "Update",
            "GeneralSettings",
            "Current",
            before,
            AuditValues(dto));
        await _repository.SaveChangesAsync();
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

    private static IReadOnlyDictionary<string, object?> AuditValues(GeneralSettingsDto value) =>
        new Dictionary<string, object?>
        {
            ["Currency"] = value.Currency,
            ["DecimalPlaces"] = value.DecimalPlaces,
            ["QuantityDecimalPlaces"] = value.QuantityDecimalPlaces,
            ["DateFormat"] = value.DateFormat,
            ["TimeZone"] = value.TimeZone,
            ["DefaultLanguage"] = value.DefaultLanguage
        };
}
