using MiniStore.Domain.Entities;

namespace MiniStore.Domain.Interfaces;

public interface IInventoryBalanceRepository
{
    Task<List<InventoryBalance>> GetAllAsync();
}
