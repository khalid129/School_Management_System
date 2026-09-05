namespace SchoolPortal.Domain.Common;

/// <summary>Thrown when a domain invariant would be violated (e.g. empty name, future DOB).</summary>
public sealed class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}
