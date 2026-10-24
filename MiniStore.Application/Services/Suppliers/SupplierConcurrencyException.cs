namespace MiniStore.Application.Services;

public sealed class SupplierConcurrencyException(string message) : InvalidOperationException(message);
