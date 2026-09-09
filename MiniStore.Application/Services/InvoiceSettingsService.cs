using MiniStore.Application.DTOs.Settings;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public class InvoiceSettingsService
{
    private readonly IInvoiceSettingsRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public InvoiceSettingsService(
        IInvoiceSettingsRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<InvoiceSettingsDto> GetAsync()
    {
        var settings = await _repository.GetAsync();

        if (settings == null)
        {
            settings = new InvoiceSettings();
            await _unitOfWork.ExecuteInTransactionAsync(
                () => _repository.AddAsync(settings));
        }

        return Map(settings);
    }

    public async Task UpdateAsync(InvoiceSettingsDto dto)
    {
        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var settings = await _repository.GetAsync();

            if (settings == null)
            {
                settings = new InvoiceSettings();
                await _repository.AddAsync(settings);
            }

            settings.UpdateFormats(
                dto.WholesalePrefix,
                dto.PosPrefix,
                dto.NumberLength);
        });
    }

    private static InvoiceSettingsDto Map(InvoiceSettings settings)
    {
        return new InvoiceSettingsDto
        {
            WholesalePrefix = settings.WholesalePrefix,
            PosPrefix = settings.PosPrefix,
            NumberLength = settings.NumberLength,
            NextWholesaleNumber = settings.NextWholesaleNumber,
            NextPosNumber = settings.NextPosNumber
        };
    }
}
