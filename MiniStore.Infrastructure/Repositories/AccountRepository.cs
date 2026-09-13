using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;
namespace MiniStore.Infrastructure.Repositories;
public class AccountRepository(AppDbContext context) : IAccountRepository
{
    public Task<List<Account>> GetAllAsync() => context.Accounts.OrderBy(x => x.Code).ToListAsync();
    public Task<Account?> GetByIdAsync(int id) => context.Accounts.FirstOrDefaultAsync(x => x.Id == id);
    public Task AddAsync(Account account) => context.Accounts.AddAsync(account).AsTask();
    public Task SaveChangesAsync() => context.SaveChangesAsync();
}
