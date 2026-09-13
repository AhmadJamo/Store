using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;
namespace MiniStore.Infrastructure.Repositories;
public class PaymentMethodRepository(AppDbContext context) : IPaymentMethodRepository
{
    public Task<List<PaymentMethod>> GetAllAsync() => context.PaymentMethods.OrderBy(x => x.Name).ToListAsync();
    public Task<PaymentMethod?> GetByIdAsync(int id) => context.PaymentMethods.FirstOrDefaultAsync(x => x.Id == id);
    public Task AddAsync(PaymentMethod method) => context.PaymentMethods.AddAsync(method).AsTask();
    public Task SaveChangesAsync() => context.SaveChangesAsync();
}
