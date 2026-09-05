namespace SchoolPortal.Application.Common.Exceptions;

/// <summary>
/// A requested resource does not exist (or is outside the caller's tenant, which the
/// global query filter makes indistinguishable — that is the intended isolation behaviour).
/// Mapped to HTTP 404 by the API exception handler.
/// </summary>
public sealed class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }

    public NotFoundException(string name, object key)
        : base($"\"{name}\" ({key}) was not found.") { }
}
