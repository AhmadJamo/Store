namespace MiniStore.Application.Services;

public sealed class SupplierDuplicateException(string message) : InvalidOperationException(message);
