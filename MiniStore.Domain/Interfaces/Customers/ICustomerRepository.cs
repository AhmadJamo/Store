using MiniStore.Domain.Entities;
namespace MiniStore.Domain.Interfaces;
public sealed record CustomerSearchCriteria(string? Search, bool IncludeArchived, string SortBy, bool SortDescending, int Page, int PageSize);
public interface ICustomerRepository
{
    Task<List<Customer>> GetAllAsync(bool includeArchived = false);
    Task<(List<Customer> Items, int TotalCount)> GetPagedAsync(CustomerSearchCriteria criteria);
    Task<Customer?> GetByIdAsync(int id);
    Task<bool> CodeExistsAsync(string code, int? excludedId = null);
    Task<bool> NameExistsAsync(string name, int? excludedId = null);
    Task<bool> AccountAssignedAsync(int accountId, int? excludedId = null);
    Task AddAsync(Customer customer);
}
