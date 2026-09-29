using MiniStore.Domain.Entities;

namespace MiniStore.Domain.Interfaces;

public interface IFiscalPeriodRepository
{
    Task<List<FiscalPeriod>> GetAllAsync();
    Task<FiscalPeriod?> GetByIdAsync(int id);
    Task<FiscalPeriod?> FindForDateAsync(DateTime date);
    Task<bool> AnyAsync();
    Task<bool> OverlapsAsync(DateTime startDate, DateTime endDate);
    Task AddAsync(FiscalPeriod period);
    void SetOriginalRowVersion(FiscalPeriod period, byte[] rowVersion);
}
