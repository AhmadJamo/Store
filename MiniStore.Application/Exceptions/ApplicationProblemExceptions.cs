namespace MiniStore.Application.Exceptions;

/// <summary>Identifies an application resource that is absent or outside the requested workflow boundary.</summary>
public sealed class ResourceNotFoundException(string message) : InvalidOperationException(message);

/// <summary>Identifies a valid command that current business state prevents.</summary>
public sealed class BusinessConflictException(string message) : InvalidOperationException(message);

/// <summary>Identifies an optimistic version conflict that requires refreshing the resource.</summary>
public sealed class OptimisticConcurrencyException(string message) : InvalidOperationException(message);

/// <summary>Identifies a duplicate business identity or replay key.</summary>
public sealed class DuplicateResourceException(string message) : InvalidOperationException(message);