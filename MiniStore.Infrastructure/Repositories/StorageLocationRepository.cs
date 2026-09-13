using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;

namespace MiniStore.Infrastructure.Repositories;

public class StorageLocationRepository(AppDbContext context) : IStorageLocationRepository
{
    public Task<StorageLocation?> GetByIdAsync(int id) => context.StorageLocations.FirstOrDefaultAsync(x => x.Id == id);
    public Task<List<StorageLocation>> SearchAsync(int? warehouseId, string? query)
    {
        var locations = context.StorageLocations.AsQueryable();
        if (warehouseId.HasValue) locations = locations.Where(x => x.WarehouseId == warehouseId);
        if (!string.IsNullOrWhiteSpace(query))
        {
            var term = query.Trim();
            locations = locations.Where(x => x.Code.Contains(term) ||
                (x.Zone != null && x.Zone.Contains(term)) || (x.Aisle != null && x.Aisle.Contains(term)) ||
                (x.Rack != null && x.Rack.Contains(term)) || (x.Level != null && x.Level.Contains(term)) ||
                (x.Bin != null && x.Bin.Contains(term)));
        }
        return locations.OrderBy(x => x.WarehouseId).ThenBy(x => x.Code).ToListAsync();
    }

    public Task<bool> CodeExistsAsync(int warehouseId, string code) =>
        context.StorageLocations.AnyAsync(x => x.WarehouseId == warehouseId && x.Code == code);
    public Task AddAsync(StorageLocation location) => context.StorageLocations.AddAsync(location).AsTask();
    public Task SaveChangesAsync() => context.SaveChangesAsync();
}
