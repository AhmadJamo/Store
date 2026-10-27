using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Application.Dtos.Settings;
namespace MiniStore.Application.Services;
public class TaxRateService(ITaxRateRepository taxes,IAccountRepository accounts)
{
    public Task<List<TaxRate>> GetAllAsync()=>taxes.GetAllAsync();
    public async Task<List<TaxRateRowDto>> GetRowsAsync() { var accountRows=(await accounts.GetAllAsync()).ToDictionary(x=>x.Id); return (await taxes.GetAllAsync()).Select(x=>new TaxRateRowDto{Id=x.Id,Name=x.Name,Rate=x.Rate,OutputAccountName=accountRows.TryGetValue(x.OutputAccountId,out var output)?$"{output.Code} — {output.Name}":string.Empty,InputAccountName=accountRows.TryGetValue(x.InputAccountId,out var input)?$"{input.Code} — {input.Name}":string.Empty,IsPriceInclusive=x.IsPriceInclusive}).ToList(); }
    public async Task CreateAsync(string name,decimal rate,int outputAccountId,int inputAccountId,bool isPriceInclusive) { if(rate<0||rate>100) throw new ArgumentException("Tax rate must be between 0 and 100."); if(await accounts.GetByIdAsync(outputAccountId) is null||await accounts.GetByIdAsync(inputAccountId) is null) throw new InvalidOperationException("Selected tax account was not found."); await taxes.AddAsync(new TaxRate(name,rate,outputAccountId,inputAccountId,isPriceInclusive)); await taxes.SaveChangesAsync(); }
}
