namespace MiniStore.Application.Exceptions;

/// <summary>Identifies an application resource that is absent or outside the requested workflow boundary.</summary>
public class ResourceNotFoundException(string message) : InvalidOperationException(message);

/// <summary>Identifies a valid command that current business state prevents.</summary>
public class BusinessConflictException(string message) : InvalidOperationException(message);

/// <summary>Identifies an optimistic version conflict that requires refreshing the resource.</summary>
public class OptimisticConcurrencyException(string message) : InvalidOperationException(message);

/// <summary>Identifies a duplicate business identity or replay key.</summary>
public class DuplicateResourceException(string message) : InvalidOperationException(message);