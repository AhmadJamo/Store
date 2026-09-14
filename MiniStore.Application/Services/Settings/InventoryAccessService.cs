using MiniStore.Application.Dtos.Settings;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public class InventoryAccessService(
    IInventoryAccessRepository accessRepository,
    IBranchRepository branchRepository,
    IWarehouseRepository warehouseRepository,
    IUnitOfWork unitOfWork)
{
    public async Task<InventoryAccessSettingsDto> GetPageAsync()
    {
        var branches = await branchRepository.GetAllAsync();
        var warehouses = await warehouseRepository.GetAllAsync();
        var accesses = await accessRepository.GetBranchAccessesAsync();
        var terminals = await accessRepository.GetPosTerminalsAsync();
        var branchNames = branches.ToDictionary(x => x.Id, x => $"{x.Code} — {x.Name}");
        var warehouseNames = warehouses.ToDictionary(x => x.Id, x => x.Name);

        return new InventoryAccessSettingsDto
        {
            Branches = branches.Select(x => new InventoryAccessBranchDto
            {
                Id = x.Id,
                DisplayName = branchNames[x.Id]
            }).ToList(),
            Warehouses = warehouses.Select(x => new InventoryAccessWarehouseDto
            {
                Id = x.Id,
                Name = x.Name,
                AllowPosSales = x.AllowPosSales
            }).ToList(),
            BranchAccesses = accesses.Select(x => new BranchWarehouseAccessDto
            {
                BranchId = x.BranchId,
                BranchName = branchNames.GetValueOrDefault(x.BranchId, "Unknown Branch"),
                WarehouseId = x.WarehouseId,
                WarehouseName = warehouseNames.GetValueOrDefault(x.WarehouseId, "Unknown Warehouse"),
                Priority = x.Priority,
                IsDefaultForPos = x.IsDefaultForPos,
                AllowPosSales = x.AllowPosSales,
                AllowPurchases = x.AllowPurchases,
                AllowTransferOut = x.AllowTransferOut,
                AllowTransferIn = x.AllowTransferIn,
                AllowReplenishment = x.AllowReplenishment
            }).ToList(),
            PosTerminals = terminals.Select(x => new PosTerminalAccessDto
            {
                Id = x.Id,
                Name = x.Name,
                BranchId = x.BranchId,
                BranchName = branchNames.GetValueOrDefault(x.BranchId, "Unknown Branch"),
                DefaultWarehouseId = x.DefaultWarehouseId,
                DefaultWarehouseName = warehouseNames.GetValueOrDefault(x.DefaultWarehouseId, "Unknown Warehouse"),
                IsActive = x.IsActive,
                Warehouses = x.Warehouses
                    .OrderBy(w => w.Priority)
                    .ThenBy(w => w.WarehouseId)
                    .Select(w => new PosTerminalWarehouseDto
                {
                    WarehouseId = w.WarehouseId,
                    WarehouseName = warehouseNames.GetValueOrDefault(w.WarehouseId, "Unknown Warehouse"),
                    Priority = w.Priority
                }).ToList()
            }).ToList()
        };
    }

    public async Task ConfigureBranchWarehouseAsync(ConfigureBranchWarehouseDto dto)
    {
        var branch = await branchRepository.GetByIdAsync(dto.BranchId)
            ?? throw new InvalidOperationException("Branch not found.");
        if (!branch.IsActive)
            throw new InvalidOperationException("Branch is inactive.");
        var warehouse = await warehouseRepository.GetByIdAsync(dto.WarehouseId)
            ?? throw new InvalidOperationException("Warehouse not found.");
        if (dto.AllowPosSales && !warehouse.AllowPosSales)
            throw new InvalidOperationException("Enable POS sales on the warehouse before allowing it for a branch.");
        if (!dto.AllowPosSales)
        {
            var terminals = await accessRepository.GetPosTerminalsAsync();
            if (terminals.Any(x =>
                x.BranchId == dto.BranchId &&
                x.Warehouses.Any(w => w.WarehouseId == dto.WarehouseId)))
            {
                throw new InvalidOperationException(
                    "This warehouse is assigned to a POS terminal and cannot be disabled for branch POS sales.");
            }
        }

        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var all = await accessRepository.GetBranchAccessesAsync();
            if (dto.IsDefaultForPos)
            {
                foreach (var current in all.Where(x =>
                    x.BranchId == dto.BranchId && x.IsDefaultForPos))
                    current.ClearDefaultForPos();
            }

            var access = all.FirstOrDefault(x =>
                x.BranchId == dto.BranchId && x.WarehouseId == dto.WarehouseId);
            if (access is null)
            {
                access = new BranchWarehouseAccess(dto.BranchId, dto.WarehouseId);
                await accessRepository.AddBranchAccessAsync(access);
            }

            access.Configure(
                dto.BranchId,
                dto.WarehouseId,
                dto.Priority,
                dto.IsDefaultForPos,
                dto.AllowPosSales,
                dto.AllowPurchases,
                dto.AllowTransferOut,
                dto.AllowTransferIn,
                dto.AllowReplenishment);
        });
    }

    public async Task CreatePosTerminalAsync(CreatePosTerminalDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentException("POS terminal name is required.");
        var access = await RequirePosAccessAsync(dto.BranchId, dto.DefaultWarehouseId);
        var terminals = await accessRepository.GetPosTerminalsAsync();
        if (terminals.Any(x => x.BranchId == dto.BranchId &&
            x.Name.Equals(dto.Name.Trim(), StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException("A POS terminal with this name already exists in the branch.");

        var terminal = new PosTerminal(dto.Name, dto.BranchId, dto.DefaultWarehouseId);
        terminal.AddOrUpdateWarehouse(dto.DefaultWarehouseId, access.Priority);
        await accessRepository.AddPosTerminalAsync(terminal);
        await accessRepository.SaveChangesAsync();
    }

    public async Task ConfigurePosTerminalWarehouseAsync(ConfigurePosTerminalWarehouseDto dto)
    {
        var terminal = await accessRepository.GetPosTerminalAsync(dto.PosTerminalId)
            ?? throw new InvalidOperationException("POS terminal not found.");
        await RequirePosAccessAsync(terminal.BranchId, dto.WarehouseId);
        terminal.AddOrUpdateWarehouse(dto.WarehouseId, dto.Priority);
        if (dto.MakeDefault)
            terminal.ChangeDefaultWarehouse(dto.WarehouseId);
        await accessRepository.SaveChangesAsync();
    }

    public async Task<bool> CanPosSellFromWarehouseAsync(int terminalId, int warehouseId)
    {
        var terminal = await accessRepository.GetPosTerminalAsync(terminalId);
        if (terminal is null || !terminal.IsActive ||
            terminal.Warehouses.All(x => x.WarehouseId != warehouseId))
            return false;

        var access = await accessRepository.GetBranchAccessAsync(terminal.BranchId, warehouseId);
        var warehouse = await warehouseRepository.GetByIdAsync(warehouseId);
        return access?.AllowPosSales == true && warehouse?.AllowPosSales == true;
    }

    public async Task<bool> HasPosTerminalsAsync() =>
        (await accessRepository.GetPosTerminalsAsync()).Count > 0;

    private async Task<BranchWarehouseAccess> RequirePosAccessAsync(int branchId, int warehouseId)
    {
        var branch = await branchRepository.GetByIdAsync(branchId)
            ?? throw new InvalidOperationException("Branch not found.");
        if (!branch.IsActive)
            throw new InvalidOperationException("Branch is inactive.");
        var warehouse = await warehouseRepository.GetByIdAsync(warehouseId)
            ?? throw new InvalidOperationException("Warehouse not found.");
        if (!warehouse.AllowPosSales)
            throw new InvalidOperationException("Warehouse is not enabled for POS sales.");
        var access = await accessRepository.GetBranchAccessAsync(branchId, warehouseId);
        if (access?.AllowPosSales != true)
            throw new InvalidOperationException("The branch is not allowed to use this warehouse for POS sales.");
        return access;
    }
}
