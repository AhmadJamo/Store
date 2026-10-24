using MiniStore.Application.DTOs.Suppliers;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public class SupplierService(
    ISupplierRepository supplierRepository,
    IAccountRepository accountRepository,
    IProductRepository productRepository,
    SupplierProductAssignmentService assignments,
    IUnitOfWork unitOfWork)
{
    public async Task<SupplierFormPageDto?> GetFormPageAsync(int? id = null)
    {
        SupplierDto form;
        if (id.HasValue)
        {
            var supplier = await supplierRepository.GetByIdAsync(id.Value);
            if (supplier is null) return null;
            form = await MapAsync(supplier);
        }
        else form = new SupplierDto { IsActive = true };

        var accounts = (await accountRepository.GetAllAsync())
            .Where(x => x.IsActive)
            .Select(x => new MiniStore.Application.Dtos.Accounting.AccountDto
                { Id = x.Id, Code = x.Code, Name = x.Name, Type = x.Type, ParentAccountId = x.ParentAccountId })
            .OrderBy(x => x.Code).ToList();
        var products = (await productRepository.GetAllAsync(null))
            .Where(x => x.IsActive && x.ProductType != ProductType.PreparedToOrder)
            .OrderBy(x => x.Name)
            .Select(x => new SupplierProductOptionDto(x.Id, x.ProductCode, x.Name)).ToList();
        return new SupplierFormPageDto(form, accounts, products);
    }

    public async Task<SupplierListPageDto> GetPageAsync(SupplierListQueryDto query)
    {
        query.Search = string.IsNullOrWhiteSpace(query.Search) ? null : query.Search.Trim();
        query.Page = Math.Max(1, query.Page);
        query.PageSize = Math.Clamp(query.PageSize, 10, 100);
        var (items, totalCount) = await supplierRepository.GetPagedAsync(new SupplierSearchCriteria(
            query.Search, query.IncludeInactive, query.Page, query.PageSize));
        return new SupplierListPageDto(items.Select(x => new SupplierListItemDto(
            x.Id, x.Name, x.LegalName, x.ContactPerson, x.Phone, x.Email,
            x.PaymentTermsDays, x.IsActive)).ToList(), query, totalCount);
    }

    public async Task<List<SupplierDto>> GetAllAsync()
    {
        var suppliers = await supplierRepository.GetAllAsync();
        var result = new List<SupplierDto>(suppliers.Count);
        foreach (var supplier in suppliers)
            result.Add(await MapAsync(supplier));
        return result;
    }

    public async Task<SupplierDto?> GetByIdAsync(int id)
    {
        var supplier = await supplierRepository.GetByIdAsync(id);
        return supplier is null ? null : await MapAsync(supplier);
    }

    public async Task<int> CreateAsync(CreateSupplierDto dto)
    {
        await ValidateNameAsync(dto.Name);
        var supplier = new Supplier(dto.Name, dto.Phone, dto.Address);
        supplier.UpdateProfile(dto.LegalName, dto.ContactPerson, dto.Phone, dto.Email, dto.Address,
            dto.TaxNumber, dto.RegistrationNumber, dto.Website, dto.PaymentTermsDays, dto.Notes, dto.IsActive);
        await AssignAccount(supplier, dto.AccountId);

        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            await supplierRepository.AddAsync(supplier);
            await unitOfWork.FlushAsync();
            await assignments.ApplyForSupplierAsync(supplier.Id, dto.IsActive ? dto.ProductIds : []);
        });
        return supplier.Id;
    }

    public async Task UpdateAsync(int id, UpdateSupplierDto dto)
    {
        var supplier = await supplierRepository.GetByIdAsync(id)
            ?? throw new InvalidOperationException("Supplier not found.");
        if (!supplier.RowVersion.SequenceEqual(dto.RowVersion))
            throw new SupplierConcurrencyException("Supplier was modified by another user. Reload and try again.");
        await ValidateNameAsync(dto.Name, id);
        await AssignAccount(supplier, dto.AccountId);

        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            supplier.ChangeName(dto.Name);
            supplier.UpdateProfile(dto.LegalName, dto.ContactPerson, dto.Phone, dto.Email, dto.Address,
                dto.TaxNumber, dto.RegistrationNumber, dto.Website, dto.PaymentTermsDays, dto.Notes, dto.IsActive);
            await assignments.ApplyForSupplierAsync(id, dto.IsActive ? dto.ProductIds : []);
        });
    }

    public async Task DeleteAsync(int id)
    {
        var supplier = await supplierRepository.GetByIdAsync(id)
            ?? throw new InvalidOperationException("Supplier not found.");
        await supplierRepository.DeleteAsync(supplier);
        await supplierRepository.SaveChangesAsync();
    }

    private async Task ValidateNameAsync(string name, int? excludingId = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Supplier name is required.");
        if ((await supplierRepository.GetAllAsync()).Any(x => x.Id != excludingId &&
            x.Name.Equals(name.Trim(), StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException("A supplier with this name already exists.");
    }

    private async Task AssignAccount(Supplier supplier, int accountId)
    {
        if (await accountRepository.GetByIdAsync(accountId) is null)
            throw new InvalidOperationException("Selected supplier account was not found.");
        supplier.AssignPayableAccount(accountId);
    }

    private async Task<SupplierDto> MapAsync(Supplier supplier)
    {
        var productIds = await assignments.GetProductIdsForSupplierAsync(supplier.Id);
        return new SupplierDto
        {
            Id = supplier.Id, Name = supplier.Name, LegalName = supplier.LegalName,
            ContactPerson = supplier.ContactPerson, Phone = supplier.Phone, Email = supplier.Email,
            Address = supplier.Address, TaxNumber = supplier.TaxNumber,
            RegistrationNumber = supplier.RegistrationNumber, Website = supplier.Website,
            PaymentTermsDays = supplier.PaymentTermsDays, Notes = supplier.Notes,
            IsActive = supplier.IsActive, AccountId = supplier.AccountId,
            ProductIds = productIds, ProductCount = productIds.Count, RowVersion = supplier.RowVersion
        };
    }
}
