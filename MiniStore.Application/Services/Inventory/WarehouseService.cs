using MiniStore.Application.DTOs.Warehouses;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public class WarehouseService
{
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IBranchRepository _branchRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly IStorageLocationRepository _storageLocationRepository;
    private readonly IProductLocationStockRepository _productLocationStockRepository;
    private readonly IInventoryAccessRepository _inventoryAccessRepository;

    public WarehouseService(
        IWarehouseRepository warehouseRepository,
        IBranchRepository branchRepository,
        IAccountRepository accountRepository,
        IStorageLocationRepository storageLocationRepository,
        IProductLocationStockRepository productLocationStockRepository,
        IInventoryAccessRepository inventoryAccessRepository)
    {
        _warehouseRepository = warehouseRepository;
        _branchRepository = branchRepository;
        _accountRepository = accountRepository;
        _storageLocationRepository = storageLocationRepository;
        _productLocationStockRepository = productLocationStockRepository;
        _inventoryAccessRepository = inventoryAccessRepository;
    }

    public async Task<List<WarehouseDto>> GetAllAsync()
    {
        var warehouses =
            await _warehouseRepository.GetAllAsync();

        return warehouses.Select(warehouse => new WarehouseDto
        {
            Id = warehouse.Id,
            Name = warehouse.Name,
            BranchId = warehouse.BranchId,
            InventoryAccountId = warehouse.InventoryAccountId,
            Type = warehouse.Type,
            ControlMode = warehouse.ControlMode,
            PickingStrategy = warehouse.PickingStrategy,
            AllowPosSales = warehouse.AllowPosSales,
            EnforceLocationCapacity = warehouse.EnforceLocationCapacity,
            RequireSourceLocationForTransfers = warehouse.RequireSourceLocationForTransfers,
            RequireDestinationLocationForTransfers = warehouse.RequireDestinationLocationForTransfers
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
            Name = warehouse.Name,
            BranchId = warehouse.BranchId,
            InventoryAccountId = warehouse.InventoryAccountId,
            Type = warehouse.Type,
            ControlMode = warehouse.ControlMode,
            PickingStrategy = warehouse.PickingStrategy,
            AllowPosSales = warehouse.AllowPosSales,
            EnforceLocationCapacity = warehouse.EnforceLocationCapacity,
            RequireSourceLocationForTransfers = warehouse.RequireSourceLocationForTransfers,
            RequireDestinationLocationForTransfers = warehouse.RequireDestinationLocationForTransfers
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
        warehouse.ConfigureInventoryOperations(
            dto.Type,
            dto.ControlMode,
            dto.PickingStrategy,
            dto.AllowPosSales,
            dto.EnforceLocationCapacity,
            dto.RequireSourceLocationForTransfers,
            dto.RequireDestinationLocationForTransfers);

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
        await ValidateControlModeChangeAsync(warehouse, dto.ControlMode);
        await ValidatePosPolicyChangeAsync(warehouse, dto.AllowPosSales);
        warehouse.AssignAccounting(dto.BranchId, dto.InventoryAccountId);
        warehouse.ConfigureInventoryOperations(
            dto.Type,
            dto.ControlMode,
            dto.PickingStrategy,
            dto.AllowPosSales,
            dto.EnforceLocationCapacity,
            dto.RequireSourceLocationForTransfers,
            dto.RequireDestinationLocationForTransfers);

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

    private async Task ValidateControlModeChangeAsync(
        Warehouse warehouse,
        InventoryControlMode requestedMode)
    {
        if (requestedMode != InventoryControlMode.Simple ||
            warehouse.ControlMode == InventoryControlMode.Simple)
        {
            return;
        }

        var locations = await _storageLocationRepository.SearchAsync(warehouse.Id, null);
        var locationStocks = await _productLocationStockRepository.GetAllAsync();
        if (locations.Count > 0 || locationStocks.Any(stock => stock.WarehouseId == warehouse.Id))
        {
            throw new InvalidOperationException(
                "This warehouse has storage locations or allocated stock and cannot be changed to Simple.");
        }
    }

    private async Task ValidatePosPolicyChangeAsync(
        Warehouse warehouse,
        bool allowPosSales)
    {
        if (allowPosSales || !warehouse.AllowPosSales)
            return;

        var accesses = await _inventoryAccessRepository.GetBranchAccessesAsync();
        if (accesses.Any(x => x.WarehouseId == warehouse.Id && x.AllowPosSales))
        {
            throw new InvalidOperationException(
                "Remove this warehouse from branch POS access before disabling POS sales.");
        }
    }

    private async Task ValidateAccountingAssignment(int branchId, int accountId)
    {
        if (await _branchRepository.GetByIdAsync(branchId) == null)
            throw new InvalidOperationException("Selected branch was not found.");
        if (await _accountRepository.GetByIdAsync(accountId) == null)
            throw new InvalidOperationException("Selected inventory account was not found.");
    }
}
