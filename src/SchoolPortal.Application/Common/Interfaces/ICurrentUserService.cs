namespace SchoolPortal.Application.Common.Interfaces;

/// <summary>The authenticated user behind the current request, if any.</summary>
public interface ICurrentUserService
{
    /// <summary>USERS.ID of the caller, or null when unauthenticated.</summary>
    Guid? UserId { get; }
}
