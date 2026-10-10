using MiniStore.Application.Dtos.Settings;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
namespace MiniStore.Application.Services;
public sealed class PurchaseMatchingSettingsService(IPurchaseMatchingSettingsRepository repository)
{
    public async Task<PurchaseMatchingSettingsDto> GetAsync()
    {
        var value = await repository.GetAsync();
        return value is null ? new() : new() { QuantityTolerancePercent = value.QuantityTolerancePercent, PriceTolerancePercent = value.PriceTolerancePercent };
    }
    public async Task UpdateAsync(PurchaseMatchingSettingsDto dto)
    {
        var value = await repository.GetAsync();
        if (value is null) await repository.AddAsync(new PurchaseMatchingSettings(dto.QuantityTolerancePercent, dto.PriceTolerancePercent));
        else value.Configure(dto.QuantityTolerancePercent, dto.PriceTolerancePercent);
        await repository.SaveChangesAsync();
    }
}
