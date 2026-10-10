namespace MiniStore.Application.Dtos.Settings;
public sealed class PurchaseMatchingSettingsDto
{
    public decimal QuantityTolerancePercent { get; set; }
    public decimal PriceTolerancePercent { get; set; }
}
