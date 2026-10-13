using MiniStore.Application.Dtos.Settings;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public sealed record ResolvedExchangeRate(string CurrencyCode, string CompanyCurrencyCode, decimal Rate, DateOnly RateDate, string Source)
{
    public decimal Convert(decimal amount) => decimal.Round(amount * Rate, 2, MidpointRounding.AwayFromZero);
}

public sealed class CurrencyExchangeRateService(
    ICurrencyExchangeRateRepository rates, IGeneralSettingsRepository generalSettings,
    IUnitOfWork unitOfWork, ICurrentUserService currentUser)
{
    public async Task<CurrencyExchangeRatePageDto> GetPageAsync(SaveCurrencyExchangeRateDto? form = null)
    {
        var companyCurrency = await GetCompanyCurrencyAsync();
        return new CurrencyExchangeRatePageDto
        {
            CompanyCurrencyCode = companyCurrency,
            Form = form ?? new SaveCurrencyExchangeRateDto { EffectiveDate = DateOnly.FromDateTime(DateTime.Today) },
            Rates = (await rates.GetAllAsync()).Select(x => new CurrencyExchangeRateDto
            {
                CurrencyCode = x.CurrencyCode, EffectiveDate = x.EffectiveDate,
                CompanyCurrencyPerUnit = x.CompanyCurrencyPerUnit, Source = x.Source
            }).ToList()
        };
    }

    public async Task SaveAsync(SaveCurrencyExchangeRateDto dto)
    {
        var companyCurrency = await GetCompanyCurrencyAsync();
        var code = Normalize(dto.CurrencyCode);
        if (code == companyCurrency) throw new InvalidOperationException("The company currency always has an exchange rate of one and does not need a rate entry.");
        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var existing = await rates.GetAsync(code, dto.EffectiveDate);
            if (existing is null) await rates.AddAsync(new CurrencyExchangeRate(code, dto.EffectiveDate, dto.CompanyCurrencyPerUnit, dto.Source, currentUser.UserId));
            else existing.Update(dto.CompanyCurrencyPerUnit, dto.Source, currentUser.UserId);
        });
    }

    public async Task<ResolvedExchangeRate> ResolveAsync(string currencyCode, DateOnly date)
    {
        var companyCurrency = await GetCompanyCurrencyAsync();
        var code = Normalize(currencyCode);
        if (code == companyCurrency) return new(code, companyCurrency, 1m, date, "Company currency");
        var rate = await rates.GetLatestAsync(code, date)
            ?? throw new InvalidOperationException($"Configure an exchange rate for {code} on or before {date:yyyy-MM-dd}.");
        return new(code, companyCurrency, rate.CompanyCurrencyPerUnit, rate.EffectiveDate, rate.Source);
    }

    private async Task<string> GetCompanyCurrencyAsync() => Normalize((await generalSettings.GetAsync())?.Currency ?? "JOD");
    private static string Normalize(string? value)
    {
        var code = (value ?? string.Empty).Trim().ToUpperInvariant();
        if (code.Length != 3 || !code.All(char.IsLetter)) throw new ArgumentException("Currency code must contain exactly three letters.");
        return code;
    }
}
