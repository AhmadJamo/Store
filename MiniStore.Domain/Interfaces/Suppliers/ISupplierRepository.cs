using MiniStore.Domain.Entities;

namespace MiniStore.Domain.Interfaces;

public sealed record SupplierSearchCriteria(string? Search, bool IncludeInactive, int Page, int PageSize);

public interface ISupplierRepository
{
    Task<Supplier?> GetByIdAsync(int id);

    Task<List<Supplier>> GetAllAsync();

    Task<(List<Supplier> Items, int TotalCount)> GetPagedAsync(SupplierSearchCriteria criteria);

    Task AddAsync(Supplier supplier);

    Task DeleteAsync(Supplier supplier);

    Task SaveChangesAsync();
}
