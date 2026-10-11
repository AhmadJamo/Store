using MiniStore.Domain.Entities;

namespace MiniStore.Domain.Interfaces;

public interface IGoodsReceiptRepository
{
    Task<List<GoodsReceipt>> GetAllAsync(string? search);
    Task<List<GoodsReceipt>> GetByPurchaseOrderIdAsync(int purchaseOrderId);
    Task<GoodsReceipt?> GetByIdAsync(int id);
    Task<GoodsReceiptScan?> GetScanByIdempotencyKeyAsync(string idempotencyKey);
    Task AddAsync(GoodsReceipt receipt);
    Task AddScanAsync(GoodsReceiptScan scan);
}
