using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;
namespace MiniStore.Infrastructure.Repositories;
public sealed class PurchaseRequestRepository(AppDbContext context) : IPurchaseRequestRepository
{
    public Task<List<PurchaseRequest>> GetAllAsync(PurchaseRequestStatus? status, string? search)
    { var q=context.PurchaseRequests.Include(x=>x.Lines).AsQueryable(); if(status.HasValue)q=q.Where(x=>x.Status==status); if(!string.IsNullOrWhiteSpace(search))q=q.Where(x=>x.RequestNumber.Contains(search)); return q.OrderByDescending(x=>x.CreatedAtUtc).ToListAsync(); }
    public Task<PurchaseRequest?> GetByIdAsync(int id)=>context.PurchaseRequests.Include(x=>x.Lines).Include(x=>x.History).FirstOrDefaultAsync(x=>x.Id==id);
    public Task<PurchaseRequest?> GetActiveBySourceAsync(string sourceType,string sourceReference)=>context.PurchaseRequests
        .FirstOrDefaultAsync(x=>x.SourceType==sourceType&&x.SourceReference==sourceReference&&x.Status!=PurchaseRequestStatus.Cancelled);
    public Task<List<PurchaseRequest>> GetActiveBySourceTypeAsync(string sourceType)=>context.PurchaseRequests
        .Where(x=>x.SourceType==sourceType&&x.Status!=PurchaseRequestStatus.Cancelled).ToListAsync();
    public Task AddAsync(PurchaseRequest request)=>context.PurchaseRequests.AddAsync(request).AsTask();
    public Task SaveChangesAsync()=>context.SaveChangesAsync();
}
