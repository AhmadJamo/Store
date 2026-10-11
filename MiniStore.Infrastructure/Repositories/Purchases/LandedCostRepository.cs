using Microsoft.EntityFrameworkCore;using MiniStore.Domain.Entities;using MiniStore.Domain.Interfaces;using MiniStore.Infrastructure.Persistence;
namespace MiniStore.Infrastructure.Repositories;
public sealed class LandedCostRepository(AppDbContext context):ILandedCostRepository
{
 private IQueryable<LandedCost>Query()=>context.LandedCosts.Include(x=>x.Components).Include(x=>x.Allocations);
 public Task<List<LandedCost>>GetAllAsync()=>Query().OrderByDescending(x=>x.CreatedAtUtc).ToListAsync();
 public Task<LandedCost?>GetByIdAsync(int id)=>Query().FirstOrDefaultAsync(x=>x.Id==id);
 public Task<LandedCost?>GetByExternalReferenceAsync(string value)=>Query().FirstOrDefaultAsync(x=>x.NormalizedExternalReference==value);
 public Task AddAsync(LandedCost value)=>context.LandedCosts.AddAsync(value).AsTask();
}
