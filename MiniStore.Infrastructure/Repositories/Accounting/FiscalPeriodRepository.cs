using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;

namespace MiniStore.Infrastructure.Repositories;

public sealed class FiscalPeriodRepository(AppDbContext context) : IFiscalPeriodRepository
{
    public Task<List<FiscalPeriod>> GetAllAsync() => context.FiscalPeriods
        .OrderByDescending(x => x.StartDate).ToListAsync();
    public Task<FiscalPeriod?> GetByIdAsync(int id) => context.FiscalPeriods.SingleOrDefaultAsync(x => x.Id == id);
    public Task<FiscalPeriod?> FindForDateAsync(DateTime date) => context.FiscalPeriods
        .SingleOrDefaultAsync(x => x.StartDate <= date && x.EndDate >= date);
    public Task<bool> AnyAsync() => context.FiscalPeriods.AnyAsync();
    public Task<bool> OverlapsAsync(DateTime startDate, DateTime endDate) => context.FiscalPeriods
        .AnyAsync(x => x.StartDate <= endDate && x.EndDate >= startDate);
    public Task AddAsync(FiscalPeriod period) => context.FiscalPeriods.AddAsync(period).AsTask();
    public void SetOriginalRowVersion(FiscalPeriod period, byte[] rowVersion) =>
        context.Entry(period).Property(x => x.RowVersion).OriginalValue = rowVersion;
}
