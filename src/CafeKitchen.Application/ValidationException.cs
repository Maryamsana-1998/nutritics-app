namespace CafeKitchen.Application;

/// <summary>Raised when a use case rejects a request for a business reason.</summary>
public sealed class ValidationException(string message) : Exception(message);
