using MiniStore.Domain.Entities;

namespace MiniStore.Domain.Interfaces;

public interface IGoodsReceiptRepository
{
    Task<List<GoodsReceipt>> GetByPurchaseOrderIdAsync(int purchaseOrderId);
    Task<GoodsReceipt?> GetByIdAsync(int id);
    Task AddAsync(GoodsReceipt receipt);
}
