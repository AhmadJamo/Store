namespace MiniStore.Application.Dtos.Settings;

public sealed class CurrencyExchangeRatePageDto
{
    public string CompanyCurrencyCode { get; init; } = string.Empty;
    public SaveCurrencyExchangeRateDto Form { get; init; } = new();
    public List<CurrencyExchangeRateDto> Rates { get; init; } = [];
}
public sealed class CurrencyExchangeRateDto
{
    public string CurrencyCode { get; init; } = string.Empty;
    public DateOnly EffectiveDate { get; init; }
    public decimal CompanyCurrencyPerUnit { get; init; }
    public string Source { get; init; } = string.Empty;
}
public sealed class SaveCurrencyExchangeRateDto
{
    public string CurrencyCode { get; set; } = string.Empty;
    public DateOnly EffectiveDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    public decimal CompanyCurrencyPerUnit { get; set; }
    public string Source { get; set; } = string.Empty;
}
