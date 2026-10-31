namespace MiniStore.Application.Services;

public sealed class RoleConcurrencyException(string message) : InvalidOperationException(message);
