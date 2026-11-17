using MiniStore.Application.Exceptions;
namespace MiniStore.Application.Services;

public sealed class SupplierConcurrencyException(string message) : OptimisticConcurrencyException(message);
