using MiniStore.Domain.Entities;

namespace MiniStore.Domain.Interfaces;

public interface IGoodsReceiptRepository
{
    Task<List<GoodsReceipt>> GetAllAsync(string? search);
    Task<List<GoodsReceipt>> GetByPurchaseOrderIdAsync(int purchaseOrderId);
    Task<GoodsReceipt?> GetByIdAsync(int id);
    Task AddAsync(GoodsReceipt receipt);
}
