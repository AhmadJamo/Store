using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;
namespace MiniStore.Infrastructure.Repositories;
public sealed class SalesPriceListRepository(AppDbContext context):ISalesPriceListRepository
{
 public Task<List<SalesPriceList>> GetAllAsync(bool includeInactive=false)=>context.SalesPriceLists.Include(x=>x.Rules).Where(x=>includeInactive||x.IsActive).OrderBy(x=>x.Name).ToListAsync();
 public Task<SalesPriceList?> GetByIdAsync(int id)=>context.SalesPriceLists.Include(x=>x.Rules).SingleOrDefaultAsync(x=>x.Id==id);
 public Task<bool> CodeExistsAsync(string code,int? excludedId=null){var value=(code??"").Trim().ToUpper();return context.SalesPriceLists.AnyAsync(x=>x.Code==value&&(!excludedId.HasValue||x.Id!=excludedId));}
 public Task AddAsync(SalesPriceList priceList)=>context.SalesPriceLists.AddAsync(priceList).AsTask();
}
