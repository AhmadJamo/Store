using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;

namespace MiniStore.Infrastructure.Repositories;

public sealed class InventoryReservationRepository(AppDbContext context) : IInventoryReservationRepository
{
    public Task<InventoryReservation?> GetBySourceAsync(InventoryReservationSourceType sourceType, int sourceId) =>
        context.InventoryReservations.Include(x => x.Lines)
            .SingleOrDefaultAsync(x => x.SourceType == sourceType && x.SourceId == sourceId);
    public Task<InventoryReservation?> GetByIdAsync(long id) => context.InventoryReservations
        .Include(x => x.Lines).SingleOrDefaultAsync(x => x.Id == id);
    public Task<List<InventoryReservation>> GetAllBySourceAsync(InventoryReservationSourceType sourceType, int sourceId) =>
        context.InventoryReservations.Include(x => x.Lines)
            .Where(x => x.SourceType == sourceType && x.SourceId == sourceId)
            .OrderByDescending(x => x.CreatedAt).ThenByDescending(x => x.Id).ToListAsync();
    public Task<List<InventoryReservation>> GetAllAsync() => context.InventoryReservations.AsNoTracking()
        .Include(x => x.Lines).OrderByDescending(x => x.CreatedAt).ThenByDescending(x => x.Id).ToListAsync();
    public Task<InventoryBalance?> GetBalanceForUpdateAsync(int productId, int warehouseId, int? storageLocationId) =>
        context.InventoryBalances.SingleOrDefaultAsync(x => x.ProductId == productId &&
            x.WarehouseId == warehouseId && x.StorageLocationId == storageLocationId);
    public Task AddAsync(InventoryReservation reservation) => context.InventoryReservations.AddAsync(reservation).AsTask();
}
