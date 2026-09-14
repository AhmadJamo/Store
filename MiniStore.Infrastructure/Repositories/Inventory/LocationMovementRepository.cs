using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;

namespace MiniStore.Infrastructure.Repositories;

public class LocationMovementRepository(AppDbContext context)
    : ILocationMovementRepository
{
    public Task<List<LocationMovement>> GetAllAsync()
    {
        return context.LocationMovements
            .AsNoTracking()
            .OrderByDescending(movement => movement.CreatedAt)
            .ThenByDescending(movement => movement.Id)
            .ToListAsync();
    }

    public Task AddAsync(LocationMovement movement)
    {
        return context.LocationMovements.AddAsync(movement).AsTask();
    }
}
