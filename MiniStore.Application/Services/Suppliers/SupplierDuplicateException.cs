using MiniStore.Application.Exceptions;
namespace MiniStore.Application.Services;

public sealed class SupplierDuplicateException(string message) : DuplicateResourceException(message);
