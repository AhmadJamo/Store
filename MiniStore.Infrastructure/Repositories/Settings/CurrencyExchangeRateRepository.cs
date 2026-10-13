using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;

namespace MiniStore.Infrastructure.Repositories;

public sealed class CurrencyExchangeRateRepository(AppDbContext context) : ICurrencyExchangeRateRepository
{
    public Task<List<CurrencyExchangeRate>> GetAllAsync() => context.CurrencyExchangeRates
        .OrderBy(x => x.CurrencyCode).ThenByDescending(x => x.EffectiveDate).ToListAsync();
    public Task<CurrencyExchangeRate?> GetAsync(string currencyCode, DateOnly effectiveDate) => context.CurrencyExchangeRates
        .SingleOrDefaultAsync(x => x.CurrencyCode == currencyCode && x.EffectiveDate == effectiveDate);
    public Task<CurrencyExchangeRate?> GetLatestAsync(string currencyCode, DateOnly onDate) => context.CurrencyExchangeRates
        .Where(x => x.CurrencyCode == currencyCode && x.EffectiveDate <= onDate)
        .OrderByDescending(x => x.EffectiveDate).ThenByDescending(x => x.Id).FirstOrDefaultAsync();
    public Task AddAsync(CurrencyExchangeRate rate) => context.CurrencyExchangeRates.AddAsync(rate).AsTask();
}
