using MiniStore.Application.DTOs.Warehouses;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public class WarehouseService
{
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IBranchRepository _branchRepository;
    private readonly IAccountRepository _accountRepository;

    public WarehouseService(IWarehouseRepository warehouseRepository, IBranchRepository branchRepository, IAccountRepository accountRepository)
    {
        _warehouseRepository = warehouseRepository;
        _branchRepository = branchRepository;
        _accountRepository = accountRepository;
    }

    public async Task<List<WarehouseDto>> GetAllAsync()
    {
        var warehouses =
            await _warehouseRepository.GetAllAsync();

        return warehouses.Select(warehouse => new WarehouseDto
        {
            Id = warehouse.Id,
            Name = warehouse.Name, BranchId = warehouse.BranchId, InventoryAccountId = warehouse.InventoryAccountId
        }).ToList();
    }

    public async Task<WarehouseDto?> GetByIdAsync(int id)
    {
        var warehouse =
            await _warehouseRepository.GetByIdAsync(id);

        if (warehouse == null)
            return null;

        return new WarehouseDto
        {
            Id = warehouse.Id,
            Name = warehouse.Name, BranchId = warehouse.BranchId, InventoryAccountId = warehouse.InventoryAccountId
        };
    }

    public async Task CreateAsync(CreateWarehouseDto dto)
    {
        var warehouses =
            await _warehouseRepository.GetAllAsync();

        var existingWarehouse = warehouses
            .FirstOrDefault(x =>
                x.Name.Equals(
                    dto.Name.Trim(),
                    StringComparison.OrdinalIgnoreCase));

        if (existingWarehouse != null)
            throw new InvalidOperationException(
                "A warehouse with this name already exists.");

        await ValidateAccountingAssignment(dto.BranchId, dto.InventoryAccountId);
        var warehouse = new Warehouse(dto.Name.Trim());
        warehouse.AssignAccounting(dto.BranchId, dto.InventoryAccountId);

        await _warehouseRepository.AddAsync(warehouse);

        await _warehouseRepository.SaveChangesAsync();
    }

    public async Task UpdateAsync(
        int id,
        UpdateWarehouseDto dto)
    {
        var warehouse =
            await _warehouseRepository.GetByIdAsync(id);

        if (warehouse == null)
            throw new InvalidOperationException(
                "Warehouse not found.");

        var warehouses =
            await _warehouseRepository.GetAllAsync();

        var existingWarehouse = warehouses
            .FirstOrDefault(x =>
                x.Id != id &&
                x.Name.Equals(
                    dto.Name.Trim(),
                    StringComparison.OrdinalIgnoreCase));

        if (existingWarehouse != null)
            throw new InvalidOperationException(
                "A warehouse with this name already exists.");

        warehouse.ChangeName(dto.Name.Trim());
        await ValidateAccountingAssignment(dto.BranchId, dto.InventoryAccountId);
        warehouse.AssignAccounting(dto.BranchId, dto.InventoryAccountId);

        await _warehouseRepository.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var warehouse =
            await _warehouseRepository.GetByIdAsync(id);

        if (warehouse == null)
            throw new InvalidOperationException(
                "Warehouse not found.");

        await _warehouseRepository.DeleteAsync(warehouse);

        await _warehouseRepository.SaveChangesAsync();
    }

    private async Task ValidateAccountingAssignment(int branchId, int accountId)
    {
        if (await _branchRepository.GetByIdAsync(branchId) == null)
            throw new InvalidOperationException("Selected branch was not found.");
        if (await _accountRepository.GetByIdAsync(accountId) == null)
            throw new InvalidOperationException("Selected inventory account was not found.");
    }
}
