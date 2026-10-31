namespace MiniStore.Application.Services;

public sealed class CustomerDuplicateException(string message) : InvalidOperationException(message);
