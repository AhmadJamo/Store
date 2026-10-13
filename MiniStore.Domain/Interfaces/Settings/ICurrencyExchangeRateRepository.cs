using MiniStore.Domain.Entities;

namespace MiniStore.Domain.Interfaces;

public interface ICurrencyExchangeRateRepository
{
    Task<List<CurrencyExchangeRate>> GetAllAsync();
    Task<CurrencyExchangeRate?> GetAsync(string currencyCode, DateOnly effectiveDate);
    Task<CurrencyExchangeRate?> GetLatestAsync(string currencyCode, DateOnly onDate);
    Task AddAsync(CurrencyExchangeRate rate);
}
