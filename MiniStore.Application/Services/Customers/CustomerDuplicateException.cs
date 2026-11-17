using MiniStore.Application.Exceptions;
namespace MiniStore.Application.Services;

public sealed class CustomerDuplicateException(string message) : DuplicateResourceException(message);
