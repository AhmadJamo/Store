using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;
namespace MiniStore.Infrastructure.Repositories;
public sealed class CustomerReturnReceiptRepository(AppDbContext context) : ICustomerReturnReceiptRepository
{
    public Task<List<CustomerReturnReceipt>> GetAllAsync(string? search = null) { var q=context.CustomerReturnReceipts.Include(x=>x.Lines).AsNoTracking().AsQueryable(); if(!string.IsNullOrWhiteSpace(search)){var t=search.Trim();q=q.Where(x=>x.ReceiptNumber.Contains(t));} return q.OrderByDescending(x=>x.ReceiptDate).ThenByDescending(x=>x.Id).ToListAsync(); }
    public Task<List<CustomerReturnReceipt>> GetByRequestAsync(int requestId)=>context.CustomerReturnReceipts.Include(x=>x.Lines).AsNoTracking().Where(x=>x.CustomerReturnRequestId==requestId).OrderByDescending(x=>x.Id).ToListAsync();
    public Task<CustomerReturnReceipt?> GetByIdAsync(int id)=>context.CustomerReturnReceipts.Include(x=>x.Lines).Include(x=>x.History).SingleOrDefaultAsync(x=>x.Id==id);
    public Task<Dictionary<int,decimal>> GetActiveReceivedQuantitiesAsync(int requestId,int? excludingReceiptId=null)=>context.CustomerReturnReceiptLines.Join(context.CustomerReturnReceipts.Where(x=>x.CustomerReturnRequestId==requestId&&x.Status!=CustomerReturnReceiptStatus.Cancelled&&(!excludingReceiptId.HasValue||x.Id!=excludingReceiptId.Value)),l=>l.CustomerReturnReceiptId,r=>r.Id,(l,r)=>l).GroupBy(x=>x.CustomerReturnRequestLineId).Select(x=>new{Id=x.Key,Quantity=x.Sum(y=>y.ReceivedQuantity)}).ToDictionaryAsync(x=>x.Id,x=>x.Quantity);
    public Task AddAsync(CustomerReturnReceipt receipt)=>context.CustomerReturnReceipts.AddAsync(receipt).AsTask();
}
