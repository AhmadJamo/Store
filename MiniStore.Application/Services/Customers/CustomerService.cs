using MiniStore.Application.Dtos.Accounting;
using MiniStore.Application.Dtos.Customers;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
namespace MiniStore.Application.Services;
public class CustomerService(ICustomerRepository customers, IAccountRepository accounts, IAccountingSettingsRepository accountingSettings,
    IGeneralSettingsRepository generalSettings, ICurrencyExchangeRateRepository exchangeRates, ITaxRateRepository taxRates,
    IUnitOfWork unitOfWork, DocumentNumberService numbers)
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

    public async Task<CustomerFormPageDto> GetCreatePageAsync(CustomerFormDto? form = null) => await BuildFormPageAsync(form ?? new());

    public async Task<CustomerFormPageDto?> GetEditPageAsync(int id, CustomerFormDto? form = null)
    {
        var customer = await customers.GetByIdAsync(id); if (customer is null) return null;
        return await BuildFormPageAsync(form ?? MapForm(customer));
    }

    public async Task<CustomerDetailsDto?> GetDetailsAsync(int id)
    {
        var customer = await customers.GetByIdAsync(id); if (customer is null) return null;
        var account = await accounts.GetByIdAsync(customer.AccountId);
        var result=MapDetails(customer, account is null ? "—" : $"{account.Code} — {account.Name}");
        if(customer.DefaultTaxRateId.HasValue) result.DefaultTaxRateName=(await taxRates.GetAllAsync()).FirstOrDefault(x=>x.Id==customer.DefaultTaxRateId)?.Name;
        result.Contacts.AddRange((await customers.GetContactsAsync(id,true)).Select(x=>new CustomerContactDto{Id=x.Id,Name=x.Name,Role=x.Role,Phone=x.Phone,Email=x.Email,IsPrimary=x.IsPrimary,IsActive=x.IsActive,RowVersion=x.RowVersion}));
        result.Addresses.AddRange((await customers.GetAddressesAsync(id,true)).Select(x=>new CustomerAddressDto{Id=x.Id,Type=x.Type,Label=x.Label,AddressLine1=x.AddressLine1,AddressLine2=x.AddressLine2,City=x.City,StateOrProvince=x.StateOrProvince,PostalCode=x.PostalCode,CountryCode=x.CountryCode,IsDefault=x.IsDefault,IsActive=x.IsActive,RowVersion=x.RowVersion}));
        return result;
    }

    public async Task<int> CreateAsync(CustomerFormDto dto)
    {
        Customer? created = null;
        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var customerCode=await numbers.GenerateAsync(DocumentNumberType.Customer, DateTime.Today);
            var selection=await ResolveCreateAccountAsync(dto,customerCode); dto.AccountId=selection.Id;
            if(selection.NewAccount is null) await ValidateAsync(dto,null);
            else await ValidateBusinessDefaultsAsync(dto,null);
            created = selection.NewAccount is null?new Customer(customerCode,dto.Name,dto.AccountId,dto.Type):new Customer(customerCode,dto.Name,selection.NewAccount,dto.Type);
            Apply(created, dto); await customers.AddAsync(created);
        });
        return created!.Id;
    }

    public async Task UpdateAsync(CustomerFormDto dto)
    {
        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var customer = await GetCurrentAsync(dto.Id, dto.RowVersion);
            if((await accountingSettings.GetAsync())?.CustomerReceivableMode is not null and not CustomerReceivableAccountMode.ManualSelection) dto.AccountId=customer.AccountId;
            await ValidateAsync(dto, dto.Id);
            Apply(customer, dto); customer.ChangeReceivableAccount(dto.AccountId);
        });
    }

    public async Task ArchiveAsync(CustomerStatusCommandDto dto) => await ChangeStatusAsync(dto, true);
    public async Task RestoreAsync(CustomerStatusCommandDto dto) => await ChangeStatusAsync(dto, false);

    public async Task SaveContactAsync(CustomerContactCommandDto dto)=>await unitOfWork.ExecuteInTransactionAsync(async()=>
    {
        _=await customers.GetByIdAsync(dto.CustomerId)??throw new InvalidOperationException("Customer was not found.");
        CustomerContact contact;
        if(dto.Id==0){contact=new CustomerContact(dto.CustomerId,dto.Name,dto.Role,dto.Phone,dto.Email,dto.IsPrimary);await customers.AddContactAsync(contact);}
        else {contact=await customers.GetContactAsync(dto.CustomerId,dto.Id)??throw new InvalidOperationException("Customer contact was not found."); EnsureVersion(contact.RowVersion,dto.RowVersion); contact.Update(dto.Name,dto.Role,dto.Phone,dto.Email,dto.IsPrimary);}
        if(dto.IsPrimary) foreach(var other in (await customers.GetContactsAsync(dto.CustomerId)).Where(other=>other.Id!=contact.Id&&other.IsPrimary)) other.RemovePrimary();
    });
    public async Task SaveAddressAsync(CustomerAddressCommandDto dto)=>await unitOfWork.ExecuteInTransactionAsync(async()=>
    {
        _=await customers.GetByIdAsync(dto.CustomerId)??throw new InvalidOperationException("Customer was not found.");
        CustomerAddress address;
        if(dto.Id==0){address=new CustomerAddress(dto.CustomerId,dto.Type,dto.Label,dto.AddressLine1,dto.AddressLine2,dto.City,dto.StateOrProvince,dto.PostalCode,dto.CountryCode,dto.IsDefault);await customers.AddAddressAsync(address);}
        else {address=await customers.GetAddressAsync(dto.CustomerId,dto.Id)??throw new InvalidOperationException("Customer address was not found."); EnsureVersion(address.RowVersion,dto.RowVersion); address.Update(dto.Type,dto.Label,dto.AddressLine1,dto.AddressLine2,dto.City,dto.StateOrProvince,dto.PostalCode,dto.CountryCode,dto.IsDefault);}
        if(dto.IsDefault) foreach(var other in (await customers.GetAddressesAsync(dto.CustomerId)).Where(other=>other.Id!=address.Id&&other.Type==dto.Type&&other.IsDefault)) other.RemoveDefault();
    });
    public async Task ChangeContactStatusAsync(CustomerChildStatusCommandDto dto,bool archive)=>await unitOfWork.ExecuteInTransactionAsync(async()=>{var x=await customers.GetContactAsync(dto.CustomerId,dto.Id)??throw new InvalidOperationException("Customer contact was not found.");EnsureVersion(x.RowVersion,dto.RowVersion);if(archive)x.Archive(DateTime.UtcNow);else x.Restore();});
    public async Task ChangeAddressStatusAsync(CustomerChildStatusCommandDto dto,bool archive)=>await unitOfWork.ExecuteInTransactionAsync(async()=>{var x=await customers.GetAddressAsync(dto.CustomerId,dto.Id)??throw new InvalidOperationException("Customer address was not found.");EnsureVersion(x.RowVersion,dto.RowVersion);if(archive)x.Archive(DateTime.UtcNow);else x.Restore();});

    private static void EnsureVersion(byte[] current,byte[] submitted){if(submitted.Length==0||!current.SequenceEqual(submitted))throw new InvalidOperationException("Customer information was modified by another user. Reload and try again.");}

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
        var policy=await accountingSettings.GetAsync();
        if (policy?.CustomerReceivableMode!=CustomerReceivableAccountMode.SharedControlAccount&&await customers.AccountAssignedAsync(dto.AccountId, excludedId)) throw new InvalidOperationException("Customer receivable account is already assigned to another customer.");
        await ValidateBusinessDefaultsAsync(dto,excludedId);
    }

    private async Task ValidateBusinessDefaultsAsync(CustomerFormDto dto,int? excludedId)
    {
        if (await customers.NameExistsAsync(dto.Name, excludedId)) throw new InvalidOperationException("Customer name already exists.");
        var supportedCurrencies=await GetCurrencyOptionsAsync();
        if(!string.IsNullOrWhiteSpace(dto.PreferredCurrencyCode)&&!supportedCurrencies.Contains(dto.PreferredCurrencyCode.Trim().ToUpperInvariant()))
            throw new InvalidOperationException("Selected preferred currency is not configured.");
        if(dto.DefaultTaxRateId.HasValue&&(await taxRates.GetAllAsync()).All(x=>x.Id!=dto.DefaultTaxRateId.Value))
            throw new InvalidOperationException("Selected default tax rate was not found.");
    }

    private async Task<(int Id,Account? NewAccount)> ResolveCreateAccountAsync(CustomerFormDto dto,string customerCode)
    {
        var policy=await accountingSettings.GetAsync();
        if(policy?.CustomerReceivableMode is null or CustomerReceivableAccountMode.ManualSelection) return (dto.AccountId,null);
        if(policy.CustomerReceivableMode==CustomerReceivableAccountMode.SharedControlAccount) return (policy.CustomerReceivableControlAccountId??throw new InvalidOperationException("Configure a shared customer receivable control account before creating customers."),null);
        var parentId=policy.CustomerReceivableParentAccountId??throw new InvalidOperationException("Configure a parent receivable account before creating automatic customer accounts.");
        var parent=await accounts.GetByIdAsync(parentId)??throw new InvalidOperationException("The configured parent receivable account was not found.");
        if(!parent.IsActive||parent.Type!=AccountType.Asset) throw new InvalidOperationException("The configured parent receivable account must be an active asset account.");
        var accountCode=$"{policy.CustomerAccountCodePrefix}{customerCode}";
        if(accountCode.Length>50) throw new InvalidOperationException("The automatic customer account code exceeds 50 characters. Shorten the configured prefix.");
        var account=new Account(accountCode,dto.Name,AccountType.Asset,parentId);
        await accounts.AddAsync(account);
        return (0,account);
    }

    private async Task<List<AccountDto>> GetAccountOptionsAsync() => (await accounts.GetAllAsync())
        .Where(x => x.ParentAccountId.HasValue && x.IsActive)
        .Select(x => new AccountDto { Id = x.Id, Code = x.Code, Name = x.Name, Type = x.Type, ParentAccountId = x.ParentAccountId }).ToList();

    private async Task<List<string>> GetCurrencyOptionsAsync()
    {
        var companyCurrency=(await generalSettings.GetAsync())?.Currency??"JOD";
        return (await exchangeRates.GetAllAsync()).Select(x=>x.CurrencyCode).Append(companyCurrency)
            .Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(x=>x).ToList();
    }
    private async Task<CustomerFormPageDto> BuildFormPageAsync(CustomerFormDto form) => new()
    {
        Form=form, Accounts=await GetAccountOptionsAsync(), Currencies=await GetCurrencyOptionsAsync(),
        TaxRates=(await taxRates.GetAllAsync()).Select(x=>new CustomerTaxRateOptionDto(x.Id,x.Name,x.Rate)).ToList()
    };

    private static void Apply(Customer customer, CustomerFormDto dto) { customer.UpdateProfile(dto.Type, dto.Name, dto.LegalName, dto.Phone, dto.Email, dto.TaxNumber, dto.RegistrationNumber, dto.Website, dto.PreferredLanguage, dto.TimeZoneId, dto.Notes); customer.ConfigureCommercialDefaults(dto.PaymentTermDays,dto.PreferredCurrencyCode,dto.DefaultTaxRateId,dto.IsTaxExempt,dto.TaxExemptionReason); }
    private static CustomerFormDto MapForm(Customer x) => new() { Id = x.Id, CustomerCode = x.CustomerCode, Name = x.Name, Type = x.Type, LegalName = x.LegalName, Phone = x.Phone, Email = x.Email, TaxNumber = x.TaxNumber, RegistrationNumber = x.RegistrationNumber, Website = x.Website, PreferredLanguage = x.PreferredLanguage, TimeZoneId = x.TimeZoneId, Notes = x.Notes, AccountId = x.AccountId, PaymentTermDays=x.PaymentTermDays, PreferredCurrencyCode=x.PreferredCurrencyCode, DefaultTaxRateId=x.DefaultTaxRateId, IsTaxExempt=x.IsTaxExempt, TaxExemptionReason=x.TaxExemptionReason, RowVersion = x.RowVersion };
    private static CustomerListItemDto MapList(Customer x, string account) => new() { Id = x.Id, CustomerCode = x.CustomerCode, Type = x.Type, Name = x.Name, Phone = x.Phone, Email = x.Email, AccountName = account, IsActive = x.IsActive };
    private static CustomerDetailsDto MapDetails(Customer x, string account) => new() { Id = x.Id, CustomerCode = x.CustomerCode, Type = x.Type, Name = x.Name, Phone = x.Phone, Email = x.Email, AccountName = account, IsActive = x.IsActive, LegalName = x.LegalName, TaxNumber = x.TaxNumber, RegistrationNumber = x.RegistrationNumber, Website = x.Website, PreferredLanguage = x.PreferredLanguage, TimeZoneId = x.TimeZoneId, Notes = x.Notes, PaymentTermDays=x.PaymentTermDays,PreferredCurrencyCode=x.PreferredCurrencyCode,IsTaxExempt=x.IsTaxExempt,TaxExemptionReason=x.TaxExemptionReason, ArchivedAtUtc = x.ArchivedAtUtc, RowVersion = x.RowVersion };
}
