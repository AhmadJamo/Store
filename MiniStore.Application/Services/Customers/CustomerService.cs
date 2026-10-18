using MiniStore.Application.Dtos.Accounting;
using MiniStore.Application.Dtos.Customers;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
namespace MiniStore.Application.Services;
public class CustomerService(ICustomerRepository customers, IAccountRepository accounts, IUnitOfWork unitOfWork, DocumentNumberService numbers)
{
    public async Task<CustomerListPageDto> GetPageAsync(CustomerListQueryDto query)
    {
        query.Search = string.IsNullOrWhiteSpace(query.Search) ? null : query.Search.Trim();
        query.Page = Math.Max(1, query.Page); query.PageSize = Math.Clamp(query.PageSize, 10, 100);
        query.SortBy = query.SortBy?.Trim().ToLowerInvariant() is "code" or "type" or "status" ? query.SortBy.Trim().ToLowerInvariant() : "name";
        var (items, total) = await customers.GetPagedAsync(new CustomerSearchCriteria(query.Search, query.IncludeArchived, query.SortBy, query.SortDescending, query.Page, query.PageSize));
        var accountNames = (await accounts.GetAllAsync()).ToDictionary(x => x.Id, x => $"{x.Code} — {x.Name}");
        return new CustomerListPageDto { Query = query, TotalCount = total, Items = items.Select(x => MapList(x, accountNames.GetValueOrDefault(x.AccountId, "—"))).ToList() };
    }

    public async Task<CustomerFormPageDto> GetCreatePageAsync(CustomerFormDto? form = null) =>
        new() { Form = form ?? new(), Accounts = await GetAccountOptionsAsync() };

    public async Task<CustomerFormPageDto?> GetEditPageAsync(int id, CustomerFormDto? form = null)
    {
        var customer = await customers.GetByIdAsync(id); if (customer is null) return null;
        return new CustomerFormPageDto { Form = form ?? MapForm(customer), Accounts = await GetAccountOptionsAsync() };
    }

    public async Task<CustomerDetailsDto?> GetDetailsAsync(int id)
    {
        var customer = await customers.GetByIdAsync(id); if (customer is null) return null;
        var account = await accounts.GetByIdAsync(customer.AccountId);
        return MapDetails(customer, account is null ? "—" : $"{account.Code} — {account.Name}");
    }

    public async Task<int> CreateAsync(CustomerFormDto dto)
    {
        Customer? created = null;
        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            await ValidateAsync(dto, null);
            created = new Customer(await numbers.GenerateAsync(DocumentNumberType.Customer, DateTime.Today), dto.Name, dto.AccountId, dto.Type);
            Apply(created, dto); await customers.AddAsync(created);
        });
        return created!.Id;
    }

    public async Task UpdateAsync(CustomerFormDto dto)
    {
        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var customer = await GetCurrentAsync(dto.Id, dto.RowVersion); await ValidateAsync(dto, dto.Id);
            Apply(customer, dto); customer.ChangeReceivableAccount(dto.AccountId);
        });
    }

    public async Task ArchiveAsync(CustomerStatusCommandDto dto) => await ChangeStatusAsync(dto, true);
    public async Task RestoreAsync(CustomerStatusCommandDto dto) => await ChangeStatusAsync(dto, false);

    private async Task ChangeStatusAsync(CustomerStatusCommandDto dto, bool archive) =>
        await unitOfWork.ExecuteInTransactionAsync(async () =>
        { var customer = await GetCurrentAsync(dto.Id, dto.RowVersion); if (archive) customer.Archive(DateTime.UtcNow); else customer.Restore(); });

    private async Task<Customer> GetCurrentAsync(int id, byte[] rowVersion)
    {
        var customer = await customers.GetByIdAsync(id) ?? throw new InvalidOperationException("Customer was not found.");
        if (rowVersion.Length == 0 || !customer.RowVersion.SequenceEqual(rowVersion))
            throw new InvalidOperationException("Customer was modified by another user. Reload and try again.");
        return customer;
    }

    private async Task ValidateAsync(CustomerFormDto dto, int? excludedId)
    {
        var account = await accounts.GetByIdAsync(dto.AccountId) ?? throw new InvalidOperationException("Selected customer account was not found.");
        if (!account.ParentAccountId.HasValue) throw new InvalidOperationException("Customer receivable account must be a subaccount in the chart of accounts.");
        if (await customers.AccountAssignedAsync(dto.AccountId, excludedId)) throw new InvalidOperationException("Customer receivable account is already assigned to another customer.");
        if (await customers.NameExistsAsync(dto.Name, excludedId)) throw new InvalidOperationException("Customer name already exists.");
    }

    private async Task<List<AccountDto>> GetAccountOptionsAsync() => (await accounts.GetAllAsync())
        .Where(x => x.ParentAccountId.HasValue && x.IsActive)
        .Select(x => new AccountDto { Id = x.Id, Code = x.Code, Name = x.Name, Type = x.Type, ParentAccountId = x.ParentAccountId }).ToList();

    private static void Apply(Customer customer, CustomerFormDto dto) => customer.UpdateProfile(dto.Type, dto.Name, dto.LegalName, dto.Phone, dto.Email, dto.TaxNumber, dto.RegistrationNumber, dto.Website, dto.PreferredLanguage, dto.TimeZoneId, dto.Notes);
    private static CustomerFormDto MapForm(Customer x) => new() { Id = x.Id, CustomerCode = x.CustomerCode, Name = x.Name, Type = x.Type, LegalName = x.LegalName, Phone = x.Phone, Email = x.Email, TaxNumber = x.TaxNumber, RegistrationNumber = x.RegistrationNumber, Website = x.Website, PreferredLanguage = x.PreferredLanguage, TimeZoneId = x.TimeZoneId, Notes = x.Notes, AccountId = x.AccountId, RowVersion = x.RowVersion };
    private static CustomerListItemDto MapList(Customer x, string account) => new() { Id = x.Id, CustomerCode = x.CustomerCode, Type = x.Type, Name = x.Name, Phone = x.Phone, Email = x.Email, AccountName = account, IsActive = x.IsActive };
    private static CustomerDetailsDto MapDetails(Customer x, string account) => new() { Id = x.Id, CustomerCode = x.CustomerCode, Type = x.Type, Name = x.Name, Phone = x.Phone, Email = x.Email, AccountName = account, IsActive = x.IsActive, LegalName = x.LegalName, TaxNumber = x.TaxNumber, RegistrationNumber = x.RegistrationNumber, Website = x.Website, PreferredLanguage = x.PreferredLanguage, TimeZoneId = x.TimeZoneId, Notes = x.Notes, ArchivedAtUtc = x.ArchivedAtUtc, RowVersion = x.RowVersion };
}
