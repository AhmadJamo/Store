using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;
namespace MiniStore.Infrastructure.Repositories;
public class CustomerRepository(AppDbContext context) : ICustomerRepository
{
    public Task<List<Customer>> GetAllAsync(bool includeArchived = false) => context.Customers.Where(x => includeArchived || x.IsActive).OrderBy(x => x.Name).ToListAsync();
    public async Task<(List<Customer> Items, int TotalCount)> GetPagedAsync(CustomerSearchCriteria c)
    {
        var query = context.Customers.AsNoTracking().Where(x => c.IncludeArchived || x.IsActive);
        if (!string.IsNullOrWhiteSpace(c.Search)) { var term = c.Search.Trim(); query = query.Where(x => x.CustomerCode.Contains(term) || x.Name.Contains(term) || (x.LegalName != null && x.LegalName.Contains(term)) || (x.Phone != null && x.Phone.Contains(term)) || (x.Email != null && x.Email.Contains(term)) || (x.TaxNumber != null && x.TaxNumber.Contains(term))); }
        var total = await query.CountAsync();
        query = (c.SortBy, c.SortDescending) switch { ("code", false) => query.OrderBy(x => x.CustomerCode), ("code", true) => query.OrderByDescending(x => x.CustomerCode), ("type", false) => query.OrderBy(x => x.Type).ThenBy(x => x.Name), ("type", true) => query.OrderByDescending(x => x.Type).ThenBy(x => x.Name), ("status", false) => query.OrderByDescending(x => x.IsActive).ThenBy(x => x.Name), ("status", true) => query.OrderBy(x => x.IsActive).ThenBy(x => x.Name), (_, true) => query.OrderByDescending(x => x.Name), _ => query.OrderBy(x => x.Name) };
        return (await query.Skip((c.Page - 1) * c.PageSize).Take(c.PageSize).ToListAsync(), total);
    }
    public Task<Customer?> GetByIdAsync(int id) => context.Customers.FirstOrDefaultAsync(x => x.Id == id);
    public Task<bool> CodeExistsAsync(string code, int? excludedId = null) => context.Customers.AnyAsync(x => x.CustomerCode == code.Trim().ToUpper() && (!excludedId.HasValue || x.Id != excludedId));
    public Task<bool> NameExistsAsync(string name, int? excludedId = null) => context.Customers.AnyAsync(x => x.Name == name.Trim() && (!excludedId.HasValue || x.Id != excludedId));
    public Task<bool> AccountAssignedAsync(int accountId, int? excludedId = null) => context.Customers.AnyAsync(x => x.AccountId == accountId && (!excludedId.HasValue || x.Id != excludedId));
    public Task AddAsync(Customer customer) => context.Customers.AddAsync(customer).AsTask();
}
