using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;
namespace MiniStore.Infrastructure.Repositories;
public sealed class CustomerCreditNoteRepository(AppDbContext context):ICustomerCreditNoteRepository
{
 private IQueryable<CustomerCreditNote> Query(bool tracking=false){var q=context.CustomerCreditNotes.Include(x=>x.Lines);return tracking?q:q.AsNoTracking();}
 public Task<List<CustomerCreditNote>>GetAllAsync()=>Query().OrderByDescending(x=>x.CreditDate).ThenByDescending(x=>x.Id).ToListAsync();
 public Task<List<CustomerCreditNote>>GetByInvoiceAsync(int invoiceId)=>Query().Where(x=>x.CustomerInvoiceId==invoiceId).OrderByDescending(x=>x.Id).ToListAsync();
 public Task<CustomerCreditNote?>GetByIdAsync(int id)=>Query().SingleOrDefaultAsync(x=>x.Id==id);
 public Task<CustomerCreditNote?>GetByRequestKeyAsync(string requestKey)=>Query().SingleOrDefaultAsync(x=>x.RequestKey==requestKey);
 public Task<Dictionary<int,decimal>>GetPostedAmountsAsync(IEnumerable<int> invoiceIds){var ids=invoiceIds.Distinct().ToList();return context.CustomerCreditNoteLines.Join(context.CustomerCreditNotes.Where(x=>ids.Contains(x.CustomerInvoiceId)&&x.Status==CustomerCreditNoteStatus.Posted),l=>l.CustomerCreditNoteId,n=>n.Id,(l,n)=>new{n.CustomerInvoiceId,l.GrossAmount}).GroupBy(x=>x.CustomerInvoiceId).Select(x=>new{Id=x.Key,Amount=x.Sum(y=>y.GrossAmount)}).ToDictionaryAsync(x=>x.Id,x=>x.Amount);}
 public Task<Dictionary<int,decimal>>GetPostedCompanyAmountsAsync(IEnumerable<int> invoiceIds){var ids=invoiceIds.Distinct().ToList();return context.CustomerCreditNoteLines.Join(context.CustomerCreditNotes.Where(x=>ids.Contains(x.CustomerInvoiceId)&&x.Status==CustomerCreditNoteStatus.Posted),l=>l.CustomerCreditNoteId,n=>n.Id,(l,n)=>new{n.CustomerInvoiceId,l.CompanyGrossAmount}).GroupBy(x=>x.CustomerInvoiceId).Select(x=>new{Id=x.Key,Amount=x.Sum(y=>y.CompanyGrossAmount)}).ToDictionaryAsync(x=>x.Id,x=>x.Amount);}
 public Task AddAsync(CustomerCreditNote note)=>context.CustomerCreditNotes.AddAsync(note).AsTask();
}
