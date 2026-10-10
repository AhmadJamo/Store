using MiniStore.Domain.Entities;

namespace MiniStore.Domain.Interfaces;

public interface IGoodsReceiptReturnRepository
{
    Task<List<GoodsReceiptReturn>> GetAllAsync();
    Task<List<GoodsReceiptReturn>> GetByGoodsReceiptIdAsync(int goodsReceiptId);
    Task<GoodsReceiptReturn?> GetByIdAsync(int id);
    Task AddAsync(GoodsReceiptReturn value);
}
