using FluentValidation.Results;

namespace SchoolPortal.Application.Common.Exceptions;

/// <summary>
/// Raised by <c>ValidationBehavior</c> when one or more FluentValidation validators fail.
/// Mapped to an RFC 7807 HTTP 400 (with the per-field errors) by the API exception handler.
/// </summary>
public sealed class ValidationException : Exception
{
    public ValidationException()
        : base("One or more validation failures have occurred.")
        => Errors = new Dictionary<string, string[]>();

    public ValidationException(IEnumerable<ValidationFailure> failures) : this()
        => Errors = failures
            .GroupBy(f => f.PropertyName, f => f.ErrorMessage)
            .ToDictionary(g => g.Key, g => g.ToArray());

    public IReadOnlyDictionary<string, string[]> Errors { get; }
}
