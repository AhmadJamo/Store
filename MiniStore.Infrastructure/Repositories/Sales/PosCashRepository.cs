using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;
namespace MiniStore.Infrastructure.Repositories;
public sealed class PosCashRepository(AppDbContext context) : IPosCashRepository
{
    public Task<PosCashPolicy?> GetPolicyAsync() => context.PosCashPolicies.SingleOrDefaultAsync();
    public async Task<IReadOnlyList<PosCashDrawer>> GetDrawersAsync(int? terminalId = null) => await context.PosCashDrawers.Where(x=>!terminalId.HasValue||x.PosTerminalId==terminalId).OrderBy(x=>x.Code).ToListAsync();
    public Task<PosCashDrawer?> GetDrawerAsync(int id) => context.PosCashDrawers.SingleOrDefaultAsync(x=>x.Id==id);
    public Task<PosCashSession?> GetSessionAsync(long id) => context.PosCashSessions.Include(x=>x.Movements).Include(x=>x.Counts).SingleOrDefaultAsync(x=>x.Id==id);
    public Task<PosCashSession?> GetCurrentSessionAsync(int terminalId) => context.PosCashSessions.Include(x=>x.Movements).Include(x=>x.Counts).SingleOrDefaultAsync(x=>x.PosTerminalId==terminalId&&(x.Status==PosCashSessionStatus.Open||x.Status==PosCashSessionStatus.CountSubmitted||x.Status==PosCashSessionStatus.AwaitingApproval));
    public Task<bool> HasSourceAsync(string sourceType,string sourceReference)=>context.PosCashMovements.AnyAsync(x=>x.SourceType==sourceType&&x.SourceReference==sourceReference);
    public Task AddPolicyAsync(PosCashPolicy value)=>context.PosCashPolicies.AddAsync(value).AsTask();
    public Task AddDrawerAsync(PosCashDrawer value)=>context.PosCashDrawers.AddAsync(value).AsTask();
    public Task AddSessionAsync(PosCashSession value)=>context.PosCashSessions.AddAsync(value).AsTask();
}
