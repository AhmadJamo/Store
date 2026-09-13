using MiniStore.Domain.Entities;
namespace MiniStore.Domain.Interfaces;
public interface IBranchRepository { Task<List<Branch>> GetAllAsync(); Task<Branch?> GetByIdAsync(int id); Task AddAsync(Branch branch); Task SaveChangesAsync(); }
