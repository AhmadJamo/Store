using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;
namespace MiniStore.Infrastructure.Repositories;
public class TaxRateRepository(AppDbContext context) : ITaxRateRepository { public Task<List<TaxRate>> GetAllAsync()=>context.TaxRates.OrderBy(x=>x.Name).ToListAsync(); public Task AddAsync(TaxRate rate)=>context.TaxRates.AddAsync(rate).AsTask(); public Task SaveChangesAsync()=>context.SaveChangesAsync(); }
