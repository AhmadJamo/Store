using MiniStore.Domain.Entities;
namespace MiniStore.Domain.Interfaces;
public interface IAccountRepository { Task<List<Account>> GetAllAsync(); Task<Account?> GetByIdAsync(int id); Task AddAsync(Account account); Task SaveChangesAsync(); }
