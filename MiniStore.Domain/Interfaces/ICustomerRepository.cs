using MiniStore.Domain.Entities;
namespace MiniStore.Domain.Interfaces;
public interface ICustomerRepository { Task<List<Customer>> GetAllAsync(); Task<Customer?> GetByIdAsync(int id); Task AddAsync(Customer customer); Task SaveChangesAsync(); }
