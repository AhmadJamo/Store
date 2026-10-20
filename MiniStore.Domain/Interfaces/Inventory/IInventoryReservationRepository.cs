using MiniStore.Domain.Entities;

namespace MiniStore.Domain.Interfaces;

public interface IInventoryReservationRepository
{
    Task<InventoryReservation?> GetBySourceAsync(InventoryReservationSourceType sourceType, int sourceId);
    Task<InventoryReservation?> GetByIdAsync(long id);
    Task<List<InventoryReservation>> GetAllBySourceAsync(InventoryReservationSourceType sourceType, int sourceId);
    Task<List<InventoryReservation>> GetAllAsync();
    Task<InventoryBalance?> GetBalanceForUpdateAsync(int productId, int warehouseId, int? storageLocationId);
    Task AddAsync(InventoryReservation reservation);
}
