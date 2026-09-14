using MiniStore.Domain.Entities;

namespace MiniStore.Domain.Interfaces;

public interface ILocationMovementRepository
{
    Task<List<LocationMovement>> GetAllAsync();

    Task AddAsync(LocationMovement movement);
}
