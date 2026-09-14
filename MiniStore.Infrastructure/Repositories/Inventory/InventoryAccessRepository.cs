using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;

namespace MiniStore.Infrastructure.Repositories;

public class InventoryAccessRepository(AppDbContext context)
    : IInventoryAccessRepository
{
    public Task<List<BranchWarehouseAccess>> GetBranchAccessesAsync() =>
        context.BranchWarehouseAccesses
            .OrderBy(x => x.BranchId)
            .ThenBy(x => x.Priority)
            .ThenBy(x => x.WarehouseId)
            .ToListAsync();

    public Task<BranchWarehouseAccess?> GetBranchAccessAsync(
        int branchId,
        int warehouseId) => context.BranchWarehouseAccesses
            .FirstOrDefaultAsync(x =>
                x.BranchId == branchId && x.WarehouseId == warehouseId);

    public Task AddBranchAccessAsync(BranchWarehouseAccess access) =>
        context.BranchWarehouseAccesses.AddAsync(access).AsTask();

    public Task<List<PosTerminal>> GetPosTerminalsAsync() => context.PosTerminals
        .Include(x => x.Warehouses)
        .OrderBy(x => x.BranchId)
        .ThenBy(x => x.Name)
        .ToListAsync();

    public Task<PosTerminal?> GetPosTerminalAsync(int id) => context.PosTerminals
        .Include(x => x.Warehouses)
        .FirstOrDefaultAsync(x => x.Id == id);

    public Task AddPosTerminalAsync(PosTerminal terminal) =>
        context.PosTerminals.AddAsync(terminal).AsTask();

    public Task SaveChangesAsync() => context.SaveChangesAsync();
}
