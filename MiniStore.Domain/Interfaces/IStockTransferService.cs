using MiniStore.Domain.Commands;
using MiniStore.Domain.Entities;

namespace MiniStore.Domain.Interfaces;

public interface IStockTransferService
{
    Task<List<StockTransfer>> GetAllAsync(
        string? search,
        StockTransferStatus? status);

    Task<StockTransfer?> GetByIdAsync(
        int id);

    Task<int> CreateAsync(
        CreateStockTransferCommand command);

    Task UpdateAsync(
    int id,
    UpdateStockTransferCommand command);

    Task SubmitAsync(
        int id,
        string userId);

    Task ApproveAsync(
        int id,
        string userId);

    Task RejectAsync(
        int id,
        string userId,
        string reason);

    Task ReturnToDraftAsync(
        int id,
        string userId);

    Task PostAsync(
        int id,
        string userId);

    Task CancelAsync(
        int id,
        string userId,
        string reason);
}