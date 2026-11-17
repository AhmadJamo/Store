using MiniStore.Application.Exceptions;
namespace MiniStore.Application.Services;

public sealed class CustomerConcurrencyException(string message) : OptimisticConcurrencyException(message);
