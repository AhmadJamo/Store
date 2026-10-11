using MiniStore.Domain.Entities;
namespace MiniStore.Domain.Interfaces;
public interface ILandedCostRepository
{
    Task<List<LandedCost>> GetAllAsync();
    Task<LandedCost?> GetByIdAsync(int id);
    Task<LandedCost?> GetByExternalReferenceAsync(string normalizedReference);
    Task AddAsync(LandedCost value);
}
