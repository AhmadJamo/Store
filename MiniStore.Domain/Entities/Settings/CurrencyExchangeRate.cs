namespace MiniStore.Domain.Entities;

public sealed class CurrencyExchangeRate
{
    private CurrencyExchangeRate() { CurrencyCode = Source = CreatedByUserId = string.Empty; }

    public int Id { get; private set; }
    public string CurrencyCode { get; private set; } = string.Empty;
    public DateOnly EffectiveDate { get; private set; }
    public decimal CompanyCurrencyPerUnit { get; private set; }
    public string Source { get; private set; } = string.Empty;
    public string CreatedByUserId { get; private set; } = string.Empty;
    public DateTime CreatedAtUtc { get; private set; }
    public byte[] RowVersion { get; private set; } = [];

    public CurrencyExchangeRate(string currencyCode, DateOnly effectiveDate, decimal companyCurrencyPerUnit,
        string source, string createdByUserId)
    {
        Set(currencyCode, effectiveDate, companyCurrencyPerUnit, source, createdByUserId);
        CreatedAtUtc = DateTime.UtcNow;
    }

    public void Update(decimal companyCurrencyPerUnit, string source, string userId) =>
        Set(CurrencyCode, EffectiveDate, companyCurrencyPerUnit, source, userId);

    private void Set(string currencyCode, DateOnly effectiveDate, decimal rate, string source, string userId)
    {
        var code = (currencyCode ?? string.Empty).Trim().ToUpperInvariant();
        if (code.Length != 3 || !code.All(char.IsLetter)) throw new ArgumentException("Currency code must contain exactly three letters.");
        if (rate <= 0) throw new ArgumentException("Exchange rate must be greater than zero.");
        if (string.IsNullOrWhiteSpace(source) || source.Trim().Length > 100) throw new ArgumentException("Exchange rate source is required and cannot exceed 100 characters.");
        if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("User is required.");
        CurrencyCode = code; EffectiveDate = effectiveDate; CompanyCurrencyPerUnit = decimal.Round(rate, 12, MidpointRounding.AwayFromZero);
        Source = source.Trim(); CreatedByUserId = userId.Trim();
    }
}
