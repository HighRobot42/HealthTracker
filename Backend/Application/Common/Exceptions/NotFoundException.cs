namespace Application.Common.Exceptions;

/// <summary>
/// Thrown when a requested entity is not found.
/// Mapped to HTTP 404 by the global exception filter.
/// </summary>
public sealed class NotFoundException(string name, object key)
    : Exception($"Entity \"{name}\" ({key}) was not found.");
