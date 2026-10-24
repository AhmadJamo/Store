using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;

namespace MiniStore.Infrastructure.Repositories;

public class SupplierRepository : ISupplierRepository
{
    private readonly AppDbContext _context;

    public SupplierRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Supplier?> GetByIdAsync(int id)
    {
        return await _context.Suppliers
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<Supplier>> GetAllAsync()
    {
        return await _context.Suppliers
            .OrderBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<(List<Supplier> Items, int TotalCount)> GetPagedAsync(SupplierSearchCriteria criteria)
    {
        var query = _context.Suppliers.AsNoTracking()
            .Where(x => criteria.IncludeInactive || x.IsActive);
        if (!string.IsNullOrWhiteSpace(criteria.Search))
        {
            var term = criteria.Search.Trim();
            query = query.Where(x => x.Name.Contains(term) ||
                (x.LegalName != null && x.LegalName.Contains(term)) ||
                (x.ContactPerson != null && x.ContactPerson.Contains(term)) ||
                (x.Phone != null && x.Phone.Contains(term)) ||
                (x.Email != null && x.Email.Contains(term)) ||
                (x.TaxNumber != null && x.TaxNumber.Contains(term)));
        }

        var totalCount = await query.CountAsync();
        var items = await query.OrderBy(x => x.Name)
            .Skip((criteria.Page - 1) * criteria.PageSize)
            .Take(criteria.PageSize)
            .ToListAsync();
        return (items, totalCount);
    }

    public async Task AddAsync(Supplier supplier)
    {
        await _context.Suppliers.AddAsync(supplier);
    }

    public async Task DeleteAsync(Supplier supplier)
    {
        _context.Suppliers.Remove(supplier);

        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
