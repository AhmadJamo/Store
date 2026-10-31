namespace MiniStore.Application.Services;

public sealed class CustomerConcurrencyException(string message) : InvalidOperationException(message);
