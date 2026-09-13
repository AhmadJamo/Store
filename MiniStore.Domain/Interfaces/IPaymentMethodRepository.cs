using MiniStore.Domain.Entities;
namespace MiniStore.Domain.Interfaces;
public interface IPaymentMethodRepository { Task<List<PaymentMethod>> GetAllAsync(); Task<PaymentMethod?> GetByIdAsync(int id); Task AddAsync(PaymentMethod method); Task SaveChangesAsync(); }
