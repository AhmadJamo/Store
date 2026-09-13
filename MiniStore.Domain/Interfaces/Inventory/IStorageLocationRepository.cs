using MiniStore.Domain.Entities;

namespace MiniStore.Domain.Interfaces;

public interface IStorageLocationRepository
{
    Task<List<StorageLocation>> SearchAsync(int? warehouseId, string? query);
    Task<StorageLocation?> GetByIdAsync(int id);
    Task<bool> CodeExistsAsync(int warehouseId, string code);
    Task AddAsync(StorageLocation location);
    Task SaveChangesAsync();
}
