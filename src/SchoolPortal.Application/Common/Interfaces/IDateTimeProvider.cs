namespace SchoolPortal.Application.Common.Interfaces;

/// <summary>Abstracts the system clock so handlers and the audit interceptor stay testable.</summary>
public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}
