using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Interfaces;
using System.Data;

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
            await _context.Database.BeginTransactionAsync(
                IsolationLevel.Serializable);

        try
        {
            await operation();

            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch (DbUpdateConcurrencyException exception)
        {
            await transaction.RollbackAsync();

            throw new InvalidOperationException(
                "Inventory changed while this operation was being processed. Please review the current stock and try again.",
                exception);
        }
        catch
        {
            await transaction.RollbackAsync();

            throw;
        }
    }
}
