using MiniStore.Domain.Entities;
namespace MiniStore.Domain.Interfaces;
public sealed record BranchOperationsSnapshot(int BranchId, decimal SalesAmount, int SalesCount,
    decimal StockQuantity, decimal StockValue, int OpenTransfers, int TransferExceptions,
    int OpenCashSessions, int CashSessionsAwaitingApproval, decimal CashVariance);
public interface IBranchRepository { Task<List<Branch>> GetAllAsync(); Task<Branch?> GetByIdAsync(int id); Task<IReadOnlyList<BranchOperationsSnapshot>> GetOperationsAsync(IReadOnlyCollection<int> branchIds, DateTime fromUtc, DateTime toUtc); Task AddAsync(Branch branch); Task SaveChangesAsync(); }
