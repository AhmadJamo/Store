using MiniStore.Domain.Entities;

namespace MiniStore.Domain.Interfaces;

public interface IInventoryAccessRepository
{
    Task<List<BranchWarehouseAccess>> GetBranchAccessesAsync();
    Task<BranchWarehouseAccess?> GetBranchAccessAsync(int branchId, int warehouseId);
    Task AddBranchAccessAsync(BranchWarehouseAccess access);
    Task<List<PosTerminal>> GetPosTerminalsAsync();
    Task<PosTerminal?> GetPosTerminalAsync(int id);
    Task AddPosTerminalAsync(PosTerminal terminal);
    Task SaveChangesAsync();
}
