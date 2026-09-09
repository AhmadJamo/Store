using MiniStore.Application.DTOs.Suppliers;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public class SupplierService
{
    private readonly ISupplierRepository _supplierRepository;

    public SupplierService(
        ISupplierRepository supplierRepository)
    {
        _supplierRepository = supplierRepository;
    }

    public async Task<List<SupplierDto>> GetAllAsync()
    {
        var suppliers =
            await _supplierRepository.GetAllAsync();

        return suppliers
            .Select(supplier => new SupplierDto
            {
                Id = supplier.Id,
                Name = supplier.Name,
                Phone = supplier.Phone,
                Address = supplier.Address
            })
            .ToList();
    }

    public async Task<SupplierDto?> GetByIdAsync(int id)
    {
        var supplier =
            await _supplierRepository.GetByIdAsync(id);

        if (supplier == null)
            return null;

        return new SupplierDto
        {
            Id = supplier.Id,
            Name = supplier.Name,
            Phone = supplier.Phone,
            Address = supplier.Address
        };
    }

    public async Task CreateAsync(
        CreateSupplierDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentException(
                "Supplier name is required.");

        var suppliers =
            await _supplierRepository.GetAllAsync();

        var existingSupplier =
            suppliers.FirstOrDefault(x =>
                x.Name.Equals(
                    dto.Name.Trim(),
                    StringComparison.OrdinalIgnoreCase));

        if (existingSupplier != null)
            throw new InvalidOperationException(
                "A supplier with this name already exists.");

        var supplier = new Supplier(
            dto.Name,
            dto.Phone,
            dto.Address);

        await _supplierRepository.AddAsync(supplier);

        await _supplierRepository.SaveChangesAsync();
    }

    public async Task UpdateAsync(
        int id,
        UpdateSupplierDto dto)
    {
        var supplier =
            await _supplierRepository.GetByIdAsync(id);

        if (supplier == null)
            throw new InvalidOperationException(
                "Supplier not found.");

        var suppliers =
            await _supplierRepository.GetAllAsync();

        var existingSupplier =
            suppliers.FirstOrDefault(x =>
                x.Id != id &&
                x.Name.Equals(
                    dto.Name.Trim(),
                    StringComparison.OrdinalIgnoreCase));

        if (existingSupplier != null)
            throw new InvalidOperationException(
                "A supplier with this name already exists.");

        supplier.ChangeName(dto.Name);
        supplier.ChangePhone(dto.Phone);
        supplier.ChangeAddress(dto.Address);

        await _supplierRepository.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var supplier =
            await _supplierRepository.GetByIdAsync(id);

        if (supplier == null)
            throw new InvalidOperationException(
                "Supplier not found.");

        await _supplierRepository.DeleteAsync(supplier);

        await _supplierRepository.SaveChangesAsync();
    }
}