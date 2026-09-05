using SchoolPortal.Application.Common.Interfaces;

namespace SchoolPortal.Infrastructure.Time;

public sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
