namespace MiniStore.Domain.Interfaces;

public interface IUnitOfWork
{
    /// <summary>Flushes tracked changes inside the active UnitOfWork transaction without committing it.</summary>
    Task FlushAsync();
    Task ExecuteInTransactionAsync(
        Func<Task> operation);
}
