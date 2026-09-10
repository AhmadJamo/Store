using MiniStore.Application.DTOs.Settings;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public class GeneralSettingsService
{
    private readonly IGeneralSettingsRepository _repository;

    public GeneralSettingsService(
        IGeneralSettingsRepository repository)
    {
        _repository = repository;
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
            RowVersion = settings.RowVersion
        };
    }

    public async Task UpdateAsync(
        GeneralSettingsDto dto)
    {
        var settings = await _repository.GetAsync();

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
                dto.TimeZone);

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
        }

        await _repository.SaveChangesAsync();
    }
}