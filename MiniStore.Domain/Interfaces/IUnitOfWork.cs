namespace MiniStore.Domain.Interfaces;

public interface IUnitOfWork
{
    Task ExecuteInTransactionAsync(
        Func<Task> operation);
}