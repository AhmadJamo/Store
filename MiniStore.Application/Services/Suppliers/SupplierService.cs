using MiniStore.Application.DTOs.Suppliers;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public class SupplierService(
    ISupplierRepository supplierRepository,
    IAccountRepository accountRepository,
    SupplierProductAssignmentService assignments,
    IUnitOfWork unitOfWork)
{
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
            throw new InvalidOperationException("Supplier was modified by another user. Reload and try again.");
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
