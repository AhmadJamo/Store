using MiniStore.Application.Dtos.Settings;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
namespace MiniStore.Application.Services;
public sealed class PurchaseMatchingSettingsService(
    IPurchaseMatchingSettingsRepository repository,
    SemanticAuditEventService audit)
{
    public async Task<PurchaseMatchingSettingsDto> GetAsync()
    {
        var value = await repository.GetAsync();
        return value is null ? new() : new() { QuantityTolerancePercent = value.QuantityTolerancePercent, PriceTolerancePercent = value.PriceTolerancePercent };
    }
    public async Task UpdateAsync(PurchaseMatchingSettingsDto dto)
    {
        var value = await repository.GetAsync();
        var before = value is null ? null : AuditValues(value.QuantityTolerancePercent, value.PriceTolerancePercent);
        if (value is null) await repository.AddAsync(new PurchaseMatchingSettings(dto.QuantityTolerancePercent, dto.PriceTolerancePercent));
        else value.Configure(dto.QuantityTolerancePercent, dto.PriceTolerancePercent);
        await audit.RecordTenantSuccessAsync(
            AuditEventCategory.Configuration,
            SemanticAuditEventTypes.PurchaseMatchingSettingsChanged,
            "Update",
            "PurchaseMatchingSettings",
            "Current",
            before,
            AuditValues(dto.QuantityTolerancePercent, dto.PriceTolerancePercent));
        await repository.SaveChangesAsync();
    }

    private static IReadOnlyDictionary<string, object?> AuditValues(decimal quantity, decimal price) =>
        new Dictionary<string, object?>
        {
            ["QuantityTolerancePercent"] = quantity,
            ["PriceTolerancePercent"] = price
        };
}
