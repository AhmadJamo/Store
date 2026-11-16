using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;
namespace MiniStore.Infrastructure.Repositories;
public class BranchRepository(AppDbContext context) : IBranchRepository
{
    public Task<List<Branch>> GetAllAsync()=>context.Branches.OrderBy(x=>x.Code).ToListAsync();
    public Task<Branch?> GetByIdAsync(int id)=>context.Branches.FirstOrDefaultAsync(x=>x.Id==id);
    public async Task<IReadOnlyList<BranchOperationsSnapshot>> GetOperationsAsync(IReadOnlyCollection<int> branchIds, DateTime fromUtc, DateTime toUtc)
    {
        var ids=branchIds.Distinct().ToArray(); if(ids.Length==0)return [];
        var access=await context.BranchWarehouseAccesses.AsNoTracking().Where(x=>ids.Contains(x.BranchId)).Select(x=>new{x.BranchId,x.WarehouseId,x.Priority}).ToListAsync();
        var warehouseIds=access.Select(x=>x.WarehouseId).Distinct().ToArray();
        var terminals=await context.PosTerminals.AsNoTracking().Where(x=>ids.Contains(x.BranchId)).Select(x=>new{x.Id,x.BranchId}).ToListAsync();
        var warehouseBranch=access.GroupBy(x=>x.WarehouseId).ToDictionary(x=>x.Key,x=>x.OrderBy(y=>y.Priority).ThenBy(y=>y.BranchId).First().BranchId);
        var terminalBranch=terminals.ToDictionary(x=>x.Id,x=>x.BranchId);
        var sales=await context.Sales.AsNoTracking().Where(x=>x.CreatedAt>=fromUtc&&x.CreatedAt<toUtc).Select(x=>new{x.TotalAmount,x.PosTerminalId,x.WarehouseId}).ToListAsync();
        var stocks=await context.ProductStocks.AsNoTracking().Where(x=>warehouseIds.Contains(x.WarehouseId)).Select(x=>new{x.WarehouseId,x.Quantity,x.InventoryValue}).ToListAsync();
        var transfers=await context.StockTransfers.AsNoTracking().Where(x=>warehouseIds.Contains(x.FromWarehouseId)||warehouseIds.Contains(x.ToWarehouseId)).Select(x=>new{x.FromWarehouseId,x.ToWarehouseId,x.Status}).ToListAsync();
        var cash=await context.PosCashSessions.AsNoTracking().Where(x=>ids.Contains(x.BranchId)&&x.OpenedAtUtc>=fromUtc&&x.OpenedAtUtc<toUtc).Select(x=>new{x.BranchId,x.Status,x.Variance}).ToListAsync();
        return ids.Select(id=>{var bs=sales.Where(x=>x.PosTerminalId.HasValue?terminalBranch.GetValueOrDefault(x.PosTerminalId.Value)==id:warehouseBranch.GetValueOrDefault(x.WarehouseId)==id).ToList();var bw=access.Where(x=>x.BranchId==id).Select(x=>x.WarehouseId).ToHashSet();var st=stocks.Where(x=>bw.Contains(x.WarehouseId)).ToList();var tr=transfers.Where(x=>bw.Contains(x.FromWarehouseId)||bw.Contains(x.ToWarehouseId)).ToList();var cs=cash.Where(x=>x.BranchId==id).ToList();return new BranchOperationsSnapshot(id,bs.Sum(x=>x.TotalAmount),bs.Count,st.Sum(x=>x.Quantity),st.Sum(x=>x.InventoryValue),tr.Count(x=>x.Status is StockTransferStatus.Draft or StockTransferStatus.Submitted or StockTransferStatus.Approved or StockTransferStatus.InTransit or StockTransferStatus.PartiallyReceived),tr.Count(x=>x.Status==StockTransferStatus.ReceiptException),cs.Count(x=>x.Status is PosCashSessionStatus.Open or PosCashSessionStatus.CountSubmitted),cs.Count(x=>x.Status==PosCashSessionStatus.AwaitingApproval),cs.Where(x=>x.Variance.HasValue).Sum(x=>x.Variance!.Value));}).ToList();
    }
    public Task AddAsync(Branch branch)=>context.Branches.AddAsync(branch).AsTask();
    public Task SaveChangesAsync()=>context.SaveChangesAsync();
}
