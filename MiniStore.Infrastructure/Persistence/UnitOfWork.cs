using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public async Task ExecuteInTransactionAsync(
        Func<Task> operation)
    {
        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        try
        {
            await operation();

            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();

            throw;
        }
    }
}
