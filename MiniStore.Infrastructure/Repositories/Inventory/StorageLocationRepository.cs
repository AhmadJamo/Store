using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;

namespace MiniStore.Infrastructure.Repositories;

public class StorageLocationRepository(AppDbContext context) : IStorageLocationRepository
{
    public Task<StorageLocation?> GetByIdAsync(int id)
    {
        return context.StorageLocations.FirstOrDefaultAsync(location => location.Id == id);
    }

    public Task<List<StorageLocation>> SearchAsync(int? warehouseId, string? query)
    {
        var locations = context.StorageLocations.AsQueryable();

        if (warehouseId.HasValue)
        {
            locations = locations.Where(
                location => location.WarehouseId == warehouseId);
        }

        if (!string.IsNullOrWhiteSpace(query))
        {
            var term = query.Trim();
            locations = locations.Where(location =>
                location.Code.Contains(term) ||
                (location.Zone != null && location.Zone.Contains(term)) ||
                (location.Aisle != null && location.Aisle.Contains(term)) ||
                (location.Rack != null && location.Rack.Contains(term)) ||
                (location.Level != null && location.Level.Contains(term)) ||
                (location.Bin != null && location.Bin.Contains(term)));
        }

        return locations
            .OrderBy(location => location.WarehouseId)
            .ThenBy(location => location.Code)
            .ToListAsync();
    }

    public Task<bool> CodeExistsAsync(int warehouseId, string code)
    {
        return context.StorageLocations.AnyAsync(location =>
            location.WarehouseId == warehouseId &&
            location.Code == code);
    }

    public Task AddAsync(StorageLocation location)
    {
        return context.StorageLocations.AddAsync(location).AsTask();
    }

    public Task SaveChangesAsync()
    {
        return context.SaveChangesAsync();
    }
}
